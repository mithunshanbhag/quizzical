namespace Quizzical.Cli;

/*
 * Rough algorithm:
 *
 * 1. Show the user a choice of pre-selected topics, and capture their choice.
 * 2. Generate a list of questions for that chosen topic.
 * 3. For each question:
 *    - Show the question to the user.
 *    - Capture the user's answer.
 *    - Check if the user's answer is correct.
 *    - Show the user the correct answer if they were wrong.
 *    - Show the user an explanation of the answer.
 *    - Update the user's score.
 *    - Move on to the next question.
 * 4. When all questions complete, show the user their score.
 * 5. Ask the user if they want to play again.
 *    - If yes, go back to step 1.
 * 6. At any point, if the user wants to quit, they can do so by pressing Ctrl+C.
 */

public class SinglePlayerConsoleQuizEngine(IConfiguration config, IQuizFactory quizFactory, IEnumerable<IQuizPlayStrategy> quizPlayStrategies)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await ShowBanner(TimeSpan.FromSeconds(1));

        var play = true;

        while (play && !cancellationToken.IsCancellationRequested)
        {
            var quizConfig = GetQuizConfig();

            var quiz = await GenerateQuizAsync(quizConfig, cancellationToken);

            IQuizPlayStrategy quizPlayStrategy = quizConfig.QuestionType switch
            {
                QuestionType.MultipleChoice => quizPlayStrategies.OfType<MultipleChoiceQuizPlayStrategy>().Single(),
                QuestionType.TrueFalse => quizPlayStrategies.OfType<TrueFalseQuizPlayStrategy>().Single(),
                QuestionType.GroupableItems => quizPlayStrategies.OfType<GroupableItemsQuizPlayStrategy>().Single(),
                _ => throw new NotSupportedException($"Question type {quizConfig.QuestionType} is not supported yet.")
            };

            var quizResponse = await quizPlayStrategy.ExecuteAsync(quiz, cancellationToken);

            play = await AskUserToPlayAgain(quizResponse);
        }
    }

    private async Task<Quiz> GenerateQuizAsync(QuizConfig quizConfig, CancellationToken cancellationToken)
    {
        return await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("Generating questions...", async _ => await quizFactory.GenerateAsync(quizConfig, cancellationToken));
    }

    private static async Task ShowBanner(TimeSpan duration)
    {
        // Small hack to ensure emojis are displayed correctly in Windows Terminal
        Console.OutputEncoding = Encoding.UTF8;

        AnsiConsole.Write(new FigletText(AppConstants.AppName));
        await Task.Delay(duration);
    }

    private QuizConfig GetQuizConfig()
    {
        var quizConfig = config.GetSection(ConfigKeys.QuizConfig).Get<QuizConfig>();

        return new QuizConfig
        {
            QuestionType = quizConfig?.QuestionType ?? GetUserSelectedQuizType(),
            Topic = quizConfig?.Topic ?? GetUserSelectedTopic(),
            Keywords = [],
            NumberOfQuestions = quizConfig?.NumberOfQuestions ?? GetUserSelectedNumberOfQuestions(),
            DifficultyLevel = quizConfig?.DifficultyLevel ?? GetUserSelectedDifficultyLevel(),
            ShowAnswerHints = quizConfig?.ShowAnswerHints ?? QuizConstants.DefaultShowAnswerHints,
            QuestionTimeLimitInSecs = quizConfig?.QuestionTimeLimitInSecs ?? QuizConstants.DefaultQuestionTimeLimitInSecs
        };
    }

    private static QuestionDifficultyLevel GetUserSelectedDifficultyLevel()
    {
        AnsiConsole.Clear();

        return AnsiConsole.Prompt(
            new SelectionPrompt<QuestionDifficultyLevel>()
                .Title($"What difficulty level would you like to play? {Emoji.Known.LevelSlider}")
                .PageSize(8)
                .HighlightStyle(Color.Cyan1.ToString())
                .AddChoices(QuestionDifficultyLevel.Easy, QuestionDifficultyLevel.Medium, QuestionDifficultyLevel.Hard));
    }

    private static int GetUserSelectedNumberOfQuestions()
    {
        AnsiConsole.Clear();

        return AnsiConsole.Prompt(
            new TextPrompt<int>("How many questions would you like to answer?")
                .DefaultValue(QuizConstants.DefaultNumberOfQuestions)
                .Validate(answer => answer is < 1 or > 20
                    ? ValidationResult.Error("Please enter a number between 1 and 20.")
                    : ValidationResult.Success()));
    }

    private static QuestionType GetUserSelectedQuizType()
    {
        AnsiConsole.Clear();

        return AnsiConsole.Prompt(
            new SelectionPrompt<QuestionType>()
                .Title($"What type of quiz would you like to play? {Emoji.Known.GrinningFace}")
                .PageSize(8)
                .HighlightStyle(Color.Cyan1.ToString())
                .MoreChoicesText("[cyan](Move up and down to reveal more topics)[/]")
                .AddChoices(QuestionType.MultipleChoice, QuestionType.TrueFalse));
    }

    private static async Task<bool> AskUserToPlayAgain(QuizEvaluation quizEvaluation)
    {
        AnsiConsole.Clear();

        var totalQuestions = quizEvaluation.Evaluations.Count;
        var skippedAnswer = quizEvaluation.Evaluations.Count(qr => qr.Value.Evaluation.IsT1);
        var correctAnswers = quizEvaluation.Evaluations.Count(qr => qr.Value.Evaluation is { IsT0: true, AsT0: true });
        var incorrectAnswers = totalQuestions - correctAnswers - skippedAnswer;

        AnsiConsole.WriteLine($"Game Over! {Emoji.Known.ThumbsUp}");
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new BreakdownChart()
            .Width(60)
            .AddItem("Right Answers", correctAnswers, Color.Green)
            .AddItem("Wrong Answers", incorrectAnswers, Color.Red)
            .AddItem("Skipped Answers", skippedAnswer, Color.Yellow));

        await Task.Delay(2000);
        AnsiConsole.WriteLine();

        return AnsiConsole.Prompt(
            new ConfirmationPrompt($"Would you like to play again? {Emoji.Known.GrinningFace}"));
    }

    private static string GetUserSelectedTopic()
    {
        AnsiConsole.Clear();

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"What's your topic? {Emoji.Known.ThinkingFace}")
                .PageSize(8)
                .HighlightStyle(Color.Cyan1.ToString())
                .MoreChoicesText("[cyan](Move up and down to reveal more topics)[/]")
                .AddChoices(AppConstants.PreSelectedTopics.Keys));
    }
}