namespace Quizzical.Models;

/// <summary>
///     Represents a multiple-select question.
/// </summary>
public class MultipleSelectQuestion : Question
{
    /// <summary>
    ///     The answer choices for the question.
    /// </summary>
    public required string[] AnswerChoices { get; set; }

    /// <summary>
    ///     The indices of all correct answers in the AnswerChoices array.
    /// </summary>
    public required int[] CorrectAnswerIndices { get; set; }

    /// <inheritdoc />
    public override QuestionEvaluation Evaluate(QuestionResponse questionResponse)
    {
        if (questionResponse.Response.IsT3)
            return new QuestionEvaluation
            {
                Evaluation = new None(),
                TimeTaken = questionResponse.TimeTaken
            };

        var selectedAnswerIndices = questionResponse.Response.AsT0;

        return new QuestionEvaluation
        {
            Evaluation = selectedAnswerIndices.OrderBy(index => index)
                .SequenceEqual(CorrectAnswerIndices.OrderBy(index => index)),
            TimeTaken = questionResponse.TimeTaken
        };
    }
}