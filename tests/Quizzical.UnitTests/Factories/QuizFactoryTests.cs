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

        // Act
        var result = await sut.GenerateAsync(quizConfig);

        // Assert
        result.Config.Should().BeSameAs(quizConfig);
        result.Questions.Should().BeSameAs(questions);
        questionFactory.CallCount.Should().Be(1);
        questionFactory.LastRequest.Should().BeSameAs(quizConfig);
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

        // Act
        Func<Task> act = () => sut.GenerateAsync(quizConfig);

        // Assert
        await act.Should()
            .ThrowAsync<NotSupportedException>()
            .WithMessage($"Question type {unsupportedQuestionType} is not supported yet.");

        questionFactory.CallCount.Should().Be(0);
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

        // Act
        var result = await sut.GenerateAsync(quizConfig);

        // Assert
        result.Questions.Should().BeEmpty();
        questionFactory.CallCount.Should().Be(1);
    }

    #endregion

    private static QuizConfig CreateQuizConfig(QuestionType questionType) =>
        new()
        {
            QuestionType = questionType,
            DifficultyLevel = QuestionDifficultyLevel.Medium,
            NumberOfQuestions = 5,
            Topic = "Science",
            Keywords = [],
            ShowAnswerHints = false,
            QuestionTimeLimitInSecs = 0
        };

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
