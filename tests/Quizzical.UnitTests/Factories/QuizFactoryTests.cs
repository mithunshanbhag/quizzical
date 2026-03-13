namespace Quizzical.UnitTests.Factories;

public class QuizFactoryTests
{
    #region Positive cases

    [Theory]
    [InlineData(QuestionType.MultipleChoice)]
    [InlineData(QuestionType.TrueFalse)]
    [InlineData(QuestionType.GroupableItems)]
    public async Task GenerateAsync_WithSupportedQuestionType_ReturnsQuiz(QuestionType questionType)
    {
        // Arrange
        var quizConfig = CreateQuizConfig(questionType);
        var questions = new Question[]
        {
            new MultipleChoiceQuestion
            {
                Text = "What is 2 + 2?",
                AnswerChoices = ["3", "4", "5"],
                CorrectAnswerIndex = 1
            }
        };

        var questionFactory = new StubQuestionFactory(questions);
        var sut = new QuizFactory(questionFactory);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var result = await sut.GenerateAsync(quizConfig, cancellationToken);

        // Assert
        Assert.Same(quizConfig, result.Config);
        Assert.Same(questions, result.Questions);
        Assert.Equal(1, questionFactory.CallCount);
        Assert.Same(quizConfig, questionFactory.LastRequest);
    }

    #endregion

    #region Negative cases

    [Fact]
    public async Task GenerateAsync_WithUnsupportedQuestionType_ThrowsNotSupportedException()
    {
        // Arrange
        var unsupportedQuestionType = (QuestionType)999;
        var quizConfig = CreateQuizConfig(unsupportedQuestionType);
        var questionFactory = new StubQuestionFactory([]);
        var sut = new QuizFactory(questionFactory);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        Func<Task> act = () => sut.GenerateAsync(quizConfig, cancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<NotSupportedException>(act);
        Assert.Equal($"Question type {unsupportedQuestionType} is not supported yet.", exception.Message);
        Assert.Equal(0, questionFactory.CallCount);
    }

    #endregion

    #region Boundary and edge cases

    [Fact]
    public async Task GenerateAsync_PreservesAnEmptyQuestionSet()
    {
        // Arrange
        var quizConfig = CreateQuizConfig(QuestionType.TrueFalse);
        var questionFactory = new StubQuestionFactory([]);
        var sut = new QuizFactory(questionFactory);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var result = await sut.GenerateAsync(quizConfig, cancellationToken);

        // Assert
        Assert.Empty(result.Questions);
        Assert.Equal(1, questionFactory.CallCount);
    }

    #endregion

    private static QuizConfig CreateQuizConfig(QuestionType questionType)
    {
        return new QuizConfig
        {
            QuestionType = questionType,
            DifficultyLevel = QuestionDifficultyLevel.Medium,
            NumberOfQuestions = 5,
            Topic = "Science",
            Keywords = [],
            ShowAnswerHints = false,
            QuestionTimeLimitInSecs = 0
        };
    }

    private sealed class StubQuestionFactory(Question[] questions) : IQuestionFactory
    {
        public int CallCount { get; private set; }

        public QuizConfig? LastRequest { get; private set; }

        public Task<Question[]> GenerateAsync(QuizConfig request, CancellationToken cancellationToken = default)
        {
            CallCount++;
            LastRequest = request;
            return Task.FromResult(questions);
        }
    }
}