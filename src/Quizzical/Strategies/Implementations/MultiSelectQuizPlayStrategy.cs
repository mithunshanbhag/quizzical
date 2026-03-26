namespace Quizzical.Strategies.Implementations;

public class MultiSelectQuizPlayStrategy : SinglePlayerConsoleQuizPlayStrategyBase
{
    protected override QuestionResponse CaptureUserResponse(Question question)
    {
        var multipleSelectQuestion = (MultipleSelectQuestion)question;

        var stopwatch = Stopwatch.StartNew();

        var multiSelection = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .HighlightStyle(Color.Cyan1.ToString())
                .PageSize(10)
                .AddChoices(
                    multipleSelectQuestion.AnswerChoices.Append(
                        QuizConstants.SkipOptionText)));

        stopwatch.Stop();

        if (multiSelection.Contains(QuizConstants.SkipOptionText))
            return new QuestionResponse { QuestionType = QuestionType.MultipleSelect, Response = new None(), TimeTaken = stopwatch.Elapsed };

        var selectedIndices = multiSelection
            .Select(answer => Array.IndexOf(multipleSelectQuestion.AnswerChoices, answer))
            .ToArray();

        return new QuestionResponse { QuestionType = QuestionType.MultipleSelect, Response = selectedIndices, TimeTaken = stopwatch.Elapsed };
    }
}