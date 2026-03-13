namespace Quizzical.Strategies.Implementations;

public class MultiSelectQuizPlayStrategy(IQuizPromptService quizPromptService) : SinglePlayerConsoleQuizPlayStrategyBase(quizPromptService)
{
    protected override QuestionResponse CaptureUserResponse(Question question)
    {
        var multipleSelectQuestion = (MultipleSelectQuestion)question;

        var stopwatch = Stopwatch.StartNew();

        var multiSelection = QuizPromptService.PromptMultiSelection(
        [
            ..multipleSelectQuestion.AnswerChoices,
            QuizConstants.SkipOptionText
        ]);

        stopwatch.Stop();

        if (multiSelection.Contains(QuizConstants.SkipOptionText))
            return new QuestionResponse { QuestionType = QuestionType.MultipleSelect, Response = new None(), TimeTaken = stopwatch.Elapsed };

        var selectedIndices = multiSelection
            .Select(answer => Array.IndexOf(multipleSelectQuestion.AnswerChoices, answer))
            .ToArray();

        return new QuestionResponse { QuestionType = QuestionType.MultipleSelect, Response = selectedIndices, TimeTaken = stopwatch.Elapsed };
    }
}