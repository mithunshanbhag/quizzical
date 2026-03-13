namespace Quizzical.Strategies.Implementations;

public class TrueFalseQuizPlayStrategy(IQuizPromptService quizPromptService) : SinglePlayerConsoleQuizPlayStrategyBase(quizPromptService)
{
    protected override QuestionResponse CaptureUserResponse(Question question)
    {
        var stopwatch = Stopwatch.StartNew();

        var selectedAnswer = QuizPromptService.PromptSingleSelection(
        [
            bool.TrueString,
            bool.FalseString,
            QuizConstants.SkipOptionText
        ]);

        stopwatch.Stop();

        return selectedAnswer == QuizConstants.SkipOptionText
            ? new QuestionResponse { QuestionType = QuestionType.TrueFalse, Response = new None(), TimeTaken = stopwatch.Elapsed }
            : new QuestionResponse { QuestionType = QuestionType.TrueFalse, Response = selectedAnswer == bool.TrueString, TimeTaken = stopwatch.Elapsed };
    }
}