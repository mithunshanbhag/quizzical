namespace Quizzical.UnitTests.Models;

public class QuestionEvaluationTests
{
    #region Positive cases

    [Fact]
    public void MultipleChoiceQuestion_Evaluate_WithCorrectAnswer_ReturnsTrue()
    {
        // Arrange
        var sut = new MultipleChoiceQuestion
        {
            Text = "What is the capital of France?",
            AnswerChoices = ["Berlin", "Madrid", "Paris"],
            CorrectAnswerIndex = 2
        };

        var response = new QuestionResponse
        {
            QuestionType = QuestionType.MultipleChoice,
            Response = 2,
            TimeTaken = TimeSpan.FromSeconds(4)
        };

        // Act
        var result = sut.Evaluate(response);

        // Assert
        result.Evaluation.IsT0.Should().BeTrue();
        result.Evaluation.AsT0.Should().BeTrue();
        result.TimeTaken.Should().Be(TimeSpan.FromSeconds(4));
    }

    [Fact]
    public void TrueFalseQuestion_Evaluate_WithCorrectAnswer_ReturnsTrue()
    {
        // Arrange
        var sut = new TrueFalseQuestion
        {
            Text = "The Pacific Ocean is larger than the Atlantic Ocean.",
            CorrectAnswer = true
        };

        var response = new QuestionResponse
        {
            QuestionType = QuestionType.TrueFalse,
            Response = true,
            TimeTaken = TimeSpan.FromSeconds(2)
        };

        // Act
        var result = sut.Evaluate(response);

        // Assert
        result.Evaluation.IsT0.Should().BeTrue();
        result.Evaluation.AsT0.Should().BeTrue();
        result.TimeTaken.Should().Be(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void GroupableItemsQuestion_Evaluate_WithMatchingGroupInDifferentOrder_ReturnsTrue()
    {
        // Arrange
        var sut = new GroupableItemsQuestion
        {
            Text = "Select all prime numbers.",
            AnswerChoices = ["2", "4", "5", "8"],
            Groupable = [0, 2]
        };

        var response = new QuestionResponse
        {
            QuestionType = QuestionType.GroupableItems,
            Response = new[] { 2, 0 },
            TimeTaken = TimeSpan.FromSeconds(3)
        };

        // Act
        var result = sut.Evaluate(response);

        // Assert
        result.Evaluation.IsT0.Should().BeTrue();
        result.Evaluation.AsT0.Should().BeTrue();
    }

    #endregion

    #region Negative cases

    [Fact]
    public void MultipleChoiceQuestion_Evaluate_WithIncorrectAnswer_ReturnsFalse()
    {
        // Arrange
        var sut = new MultipleChoiceQuestion
        {
            Text = "What is 10 / 2?",
            AnswerChoices = ["3", "4", "5"],
            CorrectAnswerIndex = 2
        };

        var response = new QuestionResponse
        {
            QuestionType = QuestionType.MultipleChoice,
            Response = 1,
            TimeTaken = TimeSpan.FromSeconds(1)
        };

        // Act
        var result = sut.Evaluate(response);

        // Assert
        result.Evaluation.IsT0.Should().BeTrue();
        result.Evaluation.AsT0.Should().BeFalse();
    }

    [Fact]
    public void TrueFalseQuestion_Evaluate_WithIncorrectAnswer_ReturnsFalse()
    {
        // Arrange
        var sut = new TrueFalseQuestion
        {
            Text = "Light travels slower than sound.",
            CorrectAnswer = false
        };

        var response = new QuestionResponse
        {
            QuestionType = QuestionType.TrueFalse,
            Response = true,
            TimeTaken = TimeSpan.FromSeconds(1)
        };

        // Act
        var result = sut.Evaluate(response);

        // Assert
        result.Evaluation.IsT0.Should().BeTrue();
        result.Evaluation.AsT0.Should().BeFalse();
    }

    #endregion

    #region Boundary and edge cases

    [Fact]
    public void GroupableItemsQuestion_Evaluate_WithSkippedAnswer_ReturnsNone()
    {
        // Arrange
        var sut = new GroupableItemsQuestion
        {
            Text = "Select all mammals.",
            AnswerChoices = ["Whale", "Salmon", "Tiger"],
            Groupable = [0, 2]
        };

        var response = new QuestionResponse
        {
            QuestionType = QuestionType.GroupableItems,
            Response = new None(),
            TimeTaken = TimeSpan.FromSeconds(5)
        };

        // Act
        var result = sut.Evaluate(response);

        // Assert
        result.Evaluation.IsT1.Should().BeTrue();
        result.TimeTaken.Should().Be(TimeSpan.FromSeconds(5));
    }

    #endregion
}
