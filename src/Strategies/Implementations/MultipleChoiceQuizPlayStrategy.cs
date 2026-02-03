namespace Quizzical.Cli.Strategies.Implementations;

public class MultipleChoiceQuizPlayStrategy : SinglePlayerConsoleQuizPlayStrategyBase
{
    protected override QuestionResponse CaptureUserResponse(Question question)
    {
        var multipleChoiceQuestion = (MultipleChoiceQuestion)question;

        var stopwatch = Stopwatch.StartNew();

        var selectedAnswer = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                //.Title("Select an answer:")
                .HighlightStyle(Color.Cyan1.ToString())
                .PageSize(10)
                .AddChoices(
                    multipleChoiceQuestion.AnswerChoices.Append(
                        QuizConstants.SkipOptionText)));

        stopwatch.Stop();

        if (selectedAnswer == QuizConstants.SkipOptionText)
            return new QuestionResponse { QuestionType = QuestionType.MultipleChoice, Response = new None(), TimeTaken = stopwatch.Elapsed };

        var selectedAnswerIndex = Array.IndexOf(multipleChoiceQuestion.AnswerChoices, selectedAnswer);

        return new QuestionResponse { QuestionType = QuestionType.MultipleChoice, Response = selectedAnswerIndex, TimeTaken = stopwatch.Elapsed };
    }
}