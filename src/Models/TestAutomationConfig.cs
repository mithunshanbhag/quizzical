namespace Quizzical.Models;

public class TestAutomationConfig
{
    public QuestionType? QuestionType { get; init; }

    public string? Topic { get; init; }

    public int? NumberOfQuestions { get; init; }

    public QuestionDifficultyLevel? DifficultyLevel { get; init; }

    public string[] SingleSelectionResponses { get; init; } = [];

    public string[][] MultiSelectionResponses { get; init; } = [];

    public bool[] ConfirmationResponses { get; init; } = [];
}