namespace Quizzical.Cli.Strategies.Implementations;

public class GroupableItemsQuizPlayStrategy : SinglePlayerConsoleQuizPlayStrategyBase
{
    protected override QuestionResponse CaptureUserResponse(Question question)
    {
        var groupableItemsQuestion = (GroupableItemsQuestion)question;

        var stopwatch = Stopwatch.StartNew();

        var multiSelection = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                //.Title("Select an answer:")
                //.Required(false)
                .HighlightStyle(Color.Cyan1.ToString())
                .PageSize(10)
                .AddChoices(
                    groupableItemsQuestion.AnswerChoices.Append(
                        QuizConstants.SkipOptionText)));

        stopwatch.Stop();

        if (multiSelection.Contains(QuizConstants.SkipOptionText))
            return new QuestionResponse { QuestionType = QuestionType.GroupableItems, Response = new None(), TimeTaken = stopwatch.Elapsed };

        var selectedIndices = multiSelection
            .Select(answer => Array.IndexOf(groupableItemsQuestion.AnswerChoices, answer))
            .ToArray();

        return new QuestionResponse { QuestionType = QuestionType.GroupableItems, Response = selectedIndices, TimeTaken = stopwatch.Elapsed };
    }
}