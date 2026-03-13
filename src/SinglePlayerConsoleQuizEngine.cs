namespace Quizzical;

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

public class SinglePlayerConsoleQuizEngine(IConfiguration config, IQuizFactory quizFactory, IEnumerable<IQuizPlayStrategy> quizPlayStrategies, IQuizPromptService quizPromptService)
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
                QuestionType.MultipleSelect => quizPlayStrategies.OfType<MultiSelectQuizPlayStrategy>().Single(),
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
        if (!ConsoleMode.IsInteractive)
            return await quizFactory.GenerateAsync(quizConfig, cancellationToken);

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

    private QuestionDifficultyLevel GetUserSelectedDifficultyLevel()
    {
        return quizPromptService.PromptDifficultyLevel();
    }

    private int GetUserSelectedNumberOfQuestions()
    {
        return quizPromptService.PromptNumberOfQuestions();
    }

    private QuestionType GetUserSelectedQuizType()
    {
        return quizPromptService.PromptQuizType();
    }

    private async Task<bool> AskUserToPlayAgain(QuizEvaluation quizEvaluation)
    {
        if (ConsoleMode.IsInteractive)
            AnsiConsole.Clear();

        var totalQuestions = quizEvaluation.Evaluations.Count;
        var skippedAnswer = quizEvaluation.Evaluations.Count(qr => qr.Value.Evaluation.IsT1);
        var correctAnswers = quizEvaluation.Evaluations.Count(qr => qr.Value.Evaluation is { IsT0: true, AsT0: true });
        var incorrectAnswers = totalQuestions - correctAnswers - skippedAnswer;

        AnsiConsole.WriteLine($"Game Over! {Emoji.Known.ThumbsUp}");
        AnsiConsole.WriteLine();

        if (ConsoleMode.IsInteractive)
        {
            AnsiConsole.Write(new BreakdownChart()
                .Width(60)
                .AddItem("Right Answers", correctAnswers, Color.Green)
                .AddItem("Wrong Answers", incorrectAnswers, Color.Red)
                .AddItem("Skipped Answers", skippedAnswer, Color.Yellow));
        }
        else
        {
            AnsiConsole.WriteLine($"Right Answers: {correctAnswers}");
            AnsiConsole.WriteLine($"Wrong Answers: {incorrectAnswers}");
            AnsiConsole.WriteLine($"Skipped Answers: {skippedAnswer}");
        }

        await Task.Delay(2000);
        AnsiConsole.WriteLine();

        return quizPromptService.PromptConfirmation($"Would you like to play again? {Emoji.Known.GrinningFace}");
    }

    private string GetUserSelectedTopic()
    {
        return quizPromptService.PromptTopic(AppConstants.PreSelectedTopics.Keys.ToArray());
    }
}