namespace Quizzical.Models;

/// <summary>
///     Represents a true/false question.
/// </summary>
public class TrueFalseQuestion : Question
{
    /// <summary>
    ///     The correct answer to the true/false question.
    /// </summary>
    public required bool CorrectAnswer { get; set; }

    /// <inheritdoc />
    public override QuestionEvaluation Evaluate(QuestionResponse questionResponse)
    {
        if (questionResponse.Response.IsT3)
            return new QuestionEvaluation
            {
                Evaluation = new None(),
                TimeTaken = questionResponse.TimeTaken
            };

        var selectedAnswer = questionResponse.Response.AsT2;

        return new QuestionEvaluation
        {
            Evaluation = selectedAnswer == CorrectAnswer,
            TimeTaken = questionResponse.TimeTaken
        };
    }
}