namespace Quizzical.Models;

/// <summary>
///     Represents a question in a quiz.
/// </summary>
public abstract class Question : IFlaggable
{
    /// <summary>
    ///     The text of the question.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    ///     An optional explanation of the answer.
    /// </summary>
    public string? ExplanationText { get; set; }

    /// <summary>
    ///     An optional hint to help the user answer the question.
    /// </summary>
    public string? AnswerHint { get; set; }

    /// <summary>
    ///     Optional time limit (in seconds) for each question. A value of 0 means no time limit.
    /// </summary>
    public int? QuestionTimeLimitInSecs { get; set; }

    /// <inheritdoc />
    public bool IsFlagged { get; set; }

    /// <inheritdoc />
    public string? FlagReason { get; set; }

    /// <summary>
    ///     Evaluates the user's selected questionResponse against the correct answer.
    /// </summary>
    /// <remarks>
    ///     The type of the questionResponse parameter will depend on the type of question.
    /// </remarks>
    /// <param name="questionResponse">
    ///     The user's response to the question.
    /// </param>
    /// <returns>
    ///     True if the user's response is correct, false if it is incorrect, and null if the question is not yet answered.
    /// </returns>
    public abstract QuestionEvaluation Evaluate(QuestionResponse questionResponse);
}