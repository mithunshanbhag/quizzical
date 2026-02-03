namespace Quizzical.Cli.Models;

/// <summary>
///     Represents a groupable-items question.
/// </summary>
public class GroupableItemsQuestion : Question
{
    /// <summary>
    ///     The answer choices for the question.
    /// </summary>
    public required string[] AnswerChoices { get; set; }

    /// <summary>
    ///     The indices of the groupable items in the AnswerChoices array.
    /// </summary>
    public required int[] Groupable { get; set; }

    /// <inheritdoc />
    public override QuestionEvaluation Evaluate(QuestionResponse questionResponse)
    {
        if (questionResponse.Response.IsT3)
            return new QuestionEvaluation
            {
                Evaluation = new None(),
                TimeTaken = questionResponse.TimeTaken
            };

        var selectedGroup = questionResponse.Response.AsT0;

        return new QuestionEvaluation
        {
            Evaluation = selectedGroup.OrderBy(i => i).SequenceEqual(Groupable.OrderBy(i => i)),
            TimeTaken = questionResponse.TimeTaken
        };
    }
}