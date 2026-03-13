namespace Quizzical.Strategies.Implementations;

public class GroupableItemsQuizPlayStrategy(IQuizPromptService quizPromptService) : SinglePlayerConsoleQuizPlayStrategyBase(quizPromptService)
{
    protected override QuestionResponse CaptureUserResponse(Question question)
    {
        var groupableItemsQuestion = (GroupableItemsQuestion)question;

        var stopwatch = Stopwatch.StartNew();

        var multiSelection = QuizPromptService.PromptMultiSelection(
        [
            ..groupableItemsQuestion.AnswerChoices,
            QuizConstants.SkipOptionText
        ]);

        stopwatch.Stop();

        if (multiSelection.Contains(QuizConstants.SkipOptionText))
            return new QuestionResponse { QuestionType = QuestionType.GroupableItems, Response = new None(), TimeTaken = stopwatch.Elapsed };

        var selectedIndices = multiSelection
            .Select(answer => Array.IndexOf(groupableItemsQuestion.AnswerChoices, answer))
            .ToArray();

        return new QuestionResponse { QuestionType = QuestionType.GroupableItems, Response = selectedIndices, TimeTaken = stopwatch.Elapsed };
    }
}