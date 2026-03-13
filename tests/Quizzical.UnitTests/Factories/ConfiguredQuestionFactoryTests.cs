using Microsoft.Extensions.Configuration;
using Quizzical.Constants;

namespace Quizzical.UnitTests.Factories;

public class ConfiguredQuestionFactoryTests
{
    #region Positive cases

    [Theory]
    [InlineData(QuestionType.MultipleChoice)]
    [InlineData(QuestionType.MultipleSelect)]
    [InlineData(QuestionType.TrueFalse)]
    public async Task GenerateAsync_WithConfiguredQuestions_ReturnsRequestedSubset(QuestionType questionType)
    {
        // Arrange
        var configuration = CreateConfiguration(GetConfiguredQuestionEntries(questionType));
        var sut = new ConfiguredQuestionFactory(configuration);
        var request = CreateQuizConfig(questionType, numberOfQuestions: 1, questionTimeLimitInSecs: 45);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var result = await sut.GenerateAsync(request, cancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal(request.QuestionTimeLimitInSecs, result[0].QuestionTimeLimitInSecs);

        Assert.IsType(questionType switch
        {
            QuestionType.MultipleChoice => typeof(MultipleChoiceQuestion),
            QuestionType.MultipleSelect => typeof(MultipleSelectQuestion),
            QuestionType.TrueFalse => typeof(TrueFalseQuestion),
            _ => throw new InvalidOperationException("Unsupported question type in test setup.")
        }, result[0]);
    }

    #endregion

    #region Negative cases

    [Fact]
    public async Task GenerateAsync_WithUnsupportedQuestionType_ThrowsNotSupportedException()
    {
        // Arrange
        var configuration = CreateConfiguration([]);
        var sut = new ConfiguredQuestionFactory(configuration);
        var request = CreateQuizConfig((QuestionType)999, numberOfQuestions: 1, questionTimeLimitInSecs: 0);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        Func<Task> act = () => sut.GenerateAsync(request, cancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<NotSupportedException>(act);
        Assert.Equal($"Question type {request.QuestionType} is not supported yet.", exception.Message);
    }

    [Fact]
    public async Task GenerateAsync_WithoutConfiguredQuestions_ThrowsInvalidOperationException()
    {
        // Arrange
        var configuration = CreateConfiguration([]);
        var sut = new ConfiguredQuestionFactory(configuration);
        var request = CreateQuizConfig(QuestionType.TrueFalse, numberOfQuestions: 1, questionTimeLimitInSecs: 0);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        Func<Task> act = () => sut.GenerateAsync(request, cancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal("No configured test questions were found for question type TrueFalse.", exception.Message);
    }

    #endregion

    #region Boundary and edge cases

    [Fact]
    public async Task GenerateAsync_WhenMoreQuestionsAreRequestedThanConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var configuration = CreateConfiguration(GetConfiguredQuestionEntries(QuestionType.MultipleChoice));
        var sut = new ConfiguredQuestionFactory(configuration);
        var request = CreateQuizConfig(QuestionType.MultipleChoice, numberOfQuestions: 3, questionTimeLimitInSecs: 0);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        Func<Task> act = () => sut.GenerateAsync(request, cancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal(
            "Requested 3 configured test questions for MultipleChoice, but only 2 were supplied.",
            exception.Message);
    }

    #endregion

    private static IConfiguration CreateConfiguration(IEnumerable<KeyValuePair<string, string?>> entries)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(entries)
            .Build();
    }

    private static QuizConfig CreateQuizConfig(QuestionType questionType, int numberOfQuestions, int questionTimeLimitInSecs)
    {
        return new QuizConfig
        {
            QuestionType = questionType,
            DifficultyLevel = QuestionDifficultyLevel.Medium,
            NumberOfQuestions = numberOfQuestions,
            Topic = "Science",
            Keywords = [],
            ShowAnswerHints = false,
            QuestionTimeLimitInSecs = questionTimeLimitInSecs
        };
    }

    private static IEnumerable<KeyValuePair<string, string?>> GetConfiguredQuestionEntries(QuestionType questionType)
    {
        return questionType switch
        {
            QuestionType.MultipleChoice =>
            [
                new($"{ConfigKeys.TestQuestionData}:MultipleChoice:0:Text", "What is 2 + 2?"),
                new($"{ConfigKeys.TestQuestionData}:MultipleChoice:0:AnswerChoices:0", "3"),
                new($"{ConfigKeys.TestQuestionData}:MultipleChoice:0:AnswerChoices:1", "4"),
                new($"{ConfigKeys.TestQuestionData}:MultipleChoice:0:AnswerChoices:2", "5"),
                new($"{ConfigKeys.TestQuestionData}:MultipleChoice:0:CorrectAnswerIndex", "1"),
                new($"{ConfigKeys.TestQuestionData}:MultipleChoice:1:Text", "What is 3 + 3?"),
                new($"{ConfigKeys.TestQuestionData}:MultipleChoice:1:AnswerChoices:0", "5"),
                new($"{ConfigKeys.TestQuestionData}:MultipleChoice:1:AnswerChoices:1", "6"),
                new($"{ConfigKeys.TestQuestionData}:MultipleChoice:1:AnswerChoices:2", "7"),
                new($"{ConfigKeys.TestQuestionData}:MultipleChoice:1:CorrectAnswerIndex", "1")
            ],
            QuestionType.MultipleSelect =>
            [
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:0:Text", "Select all prime numbers."),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:0:AnswerChoices:0", "2"),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:0:AnswerChoices:1", "4"),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:0:AnswerChoices:2", "5"),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:0:CorrectAnswerIndices:0", "0"),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:0:CorrectAnswerIndices:1", "2"),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:1:Text", "Select all mammals."),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:1:AnswerChoices:0", "Whale"),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:1:AnswerChoices:1", "Salmon"),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:1:AnswerChoices:2", "Tiger"),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:1:CorrectAnswerIndices:0", "0"),
                new($"{ConfigKeys.TestQuestionData}:MultipleSelect:1:CorrectAnswerIndices:1", "2")
            ],
            QuestionType.TrueFalse =>
            [
                new($"{ConfigKeys.TestQuestionData}:TrueFalse:0:Text", "The Earth orbits the Sun."),
                new($"{ConfigKeys.TestQuestionData}:TrueFalse:0:CorrectAnswer", "true"),
                new($"{ConfigKeys.TestQuestionData}:TrueFalse:1:Text", "Lightning never strikes the same place twice."),
                new($"{ConfigKeys.TestQuestionData}:TrueFalse:1:CorrectAnswer", "false")
            ],
            _ => throw new InvalidOperationException("Unsupported question type in test setup.")
        };
    }
}