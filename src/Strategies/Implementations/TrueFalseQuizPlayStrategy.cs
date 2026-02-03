namespace Quizzical.Cli.Strategies.Implementations;

public class TrueFalseQuizPlayStrategy : SinglePlayerConsoleQuizPlayStrategyBase
{
    protected override QuestionResponse CaptureUserResponse(Question question)
    {
        var stopwatch = Stopwatch.StartNew();

        var selectedAnswer = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                //.Title("Select an answer:")
                .HighlightStyle(Color.Cyan1.ToString())
                .PageSize(10)
                .AddChoices(bool.TrueString, bool.FalseString, QuizConstants.SkipOptionText));

        stopwatch.Stop();

        return selectedAnswer == QuizConstants.SkipOptionText
            ? new QuestionResponse { QuestionType = QuestionType.TrueFalse, Response = new None(), TimeTaken = stopwatch.Elapsed }
            : new QuestionResponse { QuestionType = QuestionType.TrueFalse, Response = selectedAnswer == bool.TrueString, TimeTaken = stopwatch.Elapsed };
    }
}