namespace Quizzical.Cli.Models;

/// <summary>
///     Represents a multiple choice question.
/// </summary>
public class MultipleChoiceQuestion : Question
{
    /// <summary>
    ///     The answer choices for the question.
    /// </summary>
    public required string[] AnswerChoices { get; set; }

    /// <summary>
    ///     The index of the correct answer in the AnswerChoices array.
    /// </summary>
    public required int CorrectAnswerIndex { get; set; }

    /// <inheritdoc />
    public override QuestionEvaluation Evaluate(QuestionResponse questionResponse)
    {
        if (questionResponse.Response.IsT3)
            return new QuestionEvaluation
            {
                Evaluation = new None(),
                TimeTaken = questionResponse.TimeTaken
            };

        var selectedAnswerIndex = questionResponse.Response.AsT1;

        return new QuestionEvaluation
        {
            Evaluation = selectedAnswerIndex == CorrectAnswerIndex,
            TimeTaken = questionResponse.TimeTaken
        };
    }
}