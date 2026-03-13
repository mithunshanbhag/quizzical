namespace Quizzical.Strategies.Implementations;

public class MultipleChoiceQuizPlayStrategy(IQuizPromptService quizPromptService) : SinglePlayerConsoleQuizPlayStrategyBase(quizPromptService)
{
    protected override QuestionResponse CaptureUserResponse(Question question)
    {
        var multipleChoiceQuestion = (MultipleChoiceQuestion)question;

        var stopwatch = Stopwatch.StartNew();

        var selectedAnswer = QuizPromptService.PromptSingleSelection(
        [
            ..multipleChoiceQuestion.AnswerChoices,
            QuizConstants.SkipOptionText
        ]);

        stopwatch.Stop();

        if (selectedAnswer == QuizConstants.SkipOptionText)
            return new QuestionResponse { QuestionType = QuestionType.MultipleChoice, Response = new None(), TimeTaken = stopwatch.Elapsed };

        var selectedAnswerIndex = Array.IndexOf(multipleChoiceQuestion.AnswerChoices, selectedAnswer);

        return new QuestionResponse { QuestionType = QuestionType.MultipleChoice, Response = selectedAnswerIndex, TimeTaken = stopwatch.Elapsed };
    }
}