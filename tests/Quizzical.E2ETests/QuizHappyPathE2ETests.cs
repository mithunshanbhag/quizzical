using System.Text.RegularExpressions;

namespace Quizzical.E2ETests;

public partial class QuizHappyPathE2ETests
{
    public static TheoryData<QuizzicalCliScenario> HappyPathScenarios =>
        new()
        {
            new QuizzicalCliScenario(
                "multiple-choice",
                """
                {
                  "QuizConfig": {
                    "QuestionType": "MultipleChoice",
                    "DifficultyLevel": "Medium",
                    "NumberOfQuestions": 1,
                    "Topic": "Space",
                    "ShowAnswerHints": false,
                    "QuestionTimeLimitInSecs": 0
                  },
                  "TestQuestionData": {
                    "MultipleChoice": [
                      {
                        "Text": "Which planet is known as the Red Planet?",
                        "AnswerChoices": [ "Venus", "Mars", "Jupiter" ],
                        "CorrectAnswerIndex": 1,
                        "ExplanationText": "Mars is known as the Red Planet."
                      }
                    ]
                  },
                  "TestAutomation": {
                    "SingleSelectionResponses": [ "Mars" ],
                    "ConfirmationResponses": [ false, false ]
                  }
                }
                """,
                "Which planet is known as the Red Planet?",
                "Mars is known as the Red Planet."),
            new QuizzicalCliScenario(
                "multi-select",
                """
                {
                  "QuizConfig": {
                    "QuestionType": "MultipleSelect",
                    "DifficultyLevel": "Medium",
                    "NumberOfQuestions": 1,
                    "Topic": "Chemistry",
                    "ShowAnswerHints": false,
                    "QuestionTimeLimitInSecs": 0
                  },
                  "TestQuestionData": {
                    "MultipleSelect": [
                      {
                        "Text": "Select the noble gases.",
                        "AnswerChoices": [ "Helium", "Oxygen", "Neon", "Nitrogen" ],
                        "CorrectAnswerIndices": [ 0, 2 ],
                        "ExplanationText": "Helium and Neon are noble gases."
                      }
                    ]
                  },
                  "TestAutomation": {
                    "MultiSelectionResponses": [ [ "Helium", "Neon" ] ],
                    "ConfirmationResponses": [ false, false ]
                  }
                }
                """,
                "Select the noble gases.",
                "Helium and Neon are noble gases."),
            new QuizzicalCliScenario(
                "true-false",
                """
                {
                  "QuizConfig": {
                    "QuestionType": "TrueFalse",
                    "DifficultyLevel": "Easy",
                    "NumberOfQuestions": 1,
                    "Topic": "Nature",
                    "ShowAnswerHints": false,
                    "QuestionTimeLimitInSecs": 0
                  },
                  "TestQuestionData": {
                    "TrueFalse": [
                      {
                        "Text": "Lightning is hotter than the surface of the sun.",
                        "CorrectAnswer": true,
                        "ExplanationText": "A lightning bolt can briefly be hotter than the sun's surface."
                      }
                    ]
                  },
                  "TestAutomation": {
                    "SingleSelectionResponses": [ "True" ],
                    "ConfirmationResponses": [ false, false ]
                  }
                }
                """,
                "Lightning is hotter than the surface of the sun.",
                "A lightning bolt can briefly be hotter than the sun's surface.")
        };

    [Theory]
    [MemberData(nameof(HappyPathScenarios))]
    public async Task RunAsync_WithDeterministicHappyPath_CompletesQuizAndShowsSummary(QuizzicalCliScenario scenario)
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var result = await QuizzicalCliRunner.RunAsync(scenario.ConfigurationJson, cancellationToken);
        var output = NormalizeOutput(result.StandardOutput);
        var error = NormalizeOutput(result.StandardError);

        // Assert
        Assert.Equal(0, result.ExitCode);
        Assert.Contains(scenario.ExpectedQuestionText, output, StringComparison.Ordinal);
        Assert.Contains("Correct", output, StringComparison.Ordinal);
        Assert.Contains(scenario.ExpectedExplanationText, output, StringComparison.Ordinal);
        Assert.Contains("Game Over!", output, StringComparison.Ordinal);
        Assert.Contains("Would you like to play again?", output, StringComparison.Ordinal);
        Assert.DoesNotContain("Unhandled exception", output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Unhandled exception", error, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeOutput(string output)
    {
        var withoutAnsi = AnsiEscapeSequenceRegex().Replace(output, string.Empty);
        return withoutAnsi.Replace("\r", string.Empty);
    }

    [GeneratedRegex(@"\x1B\[[0-?]*[ -/]*[@-~]")]
    private static partial Regex AnsiEscapeSequenceRegex();
}

public sealed record QuizzicalCliScenario(
    string Name,
    string ConfigurationJson,
    string ExpectedQuestionText,
    string ExpectedExplanationText)
{
    public override string ToString() => Name;
}