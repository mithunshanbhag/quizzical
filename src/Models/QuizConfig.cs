namespace Quizzical.Cli.Models;

/// <summary>
///     The configuration required to create a quiz.
/// </summary>
public class QuizConfig
{
    /// <summary>
    ///     The type of question. See <see cref="Constants.Enums.QuestionType" />
    /// </summary>
    public QuestionType QuestionType { get; set; }

    /// <summary>
    ///     The difficulty level of the question. See <see cref="QuestionDifficultyLevel" />
    /// </summary>
    public QuestionDifficultyLevel DifficultyLevel { get; set; }

    /// <summary>
    ///     The number of questions to return.
    /// </summary>
    public int NumberOfQuestions { get; set; }

    /// <summary>
    ///     The topic of the question.
    /// </summary>
    public required string Topic { get; set; }

    /// <summary>
    ///     The keywords associated with the question.
    /// </summary>
    public required List<string> Keywords { get; set; }

    /// <summary>
    ///     Whether to show hints to the user.
    /// </summary>
    public required bool ShowAnswerHints { get; set; }

    /// <summary>
    ///     The time limit (in seconds) for each question. A value of 0 means no time limit.
    /// </summary>
    public int QuestionTimeLimitInSecs { get; set; }
}