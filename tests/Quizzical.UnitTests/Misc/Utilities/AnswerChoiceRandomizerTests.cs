using Quizzical.Misc.Utilities;

namespace Quizzical.UnitTests.Misc.Utilities;

public class AnswerChoiceRandomizerTests
{
    [Fact]
    public void ApplyChoiceOrder_ForMultipleChoice_RemapsCorrectAnswerIndexAndPreservesEvaluation()
    {
        // Arrange
        var sut = new MultipleChoiceQuestion
        {
            Text = "What is the capital of France?",
            AnswerChoices = ["Berlin", "Madrid", "Paris", "Rome"],
            CorrectAnswerIndex = 2
        };

        // Act
        AnswerChoiceRandomizer.ApplyChoiceOrder(sut, [2, 0, 3, 1]);

        // Assert
        Assert.Equal(["Paris", "Berlin", "Rome", "Madrid"], sut.AnswerChoices);
        Assert.Equal(0, sut.CorrectAnswerIndex);

        var evaluation = sut.Evaluate(new QuestionResponse
        {
            QuestionType = QuestionType.MultipleChoice,
            Response = 0,
            TimeTaken = TimeSpan.FromSeconds(1)
        });

        Assert.True(evaluation.Evaluation.IsT0);
        Assert.True(evaluation.Evaluation.AsT0);
    }

    [Fact]
    public void ApplyChoiceOrder_ForMultipleSelect_RemapsCorrectAnswerIndicesAndPreservesEvaluation()
    {
        // Arrange
        var sut = new MultipleSelectQuestion
        {
            Text = "Select all planets with rings.",
            AnswerChoices = ["Earth", "Saturn", "Mars", "Uranus"],
            CorrectAnswerIndices = [1, 3]
        };

        // Act
        AnswerChoiceRandomizer.ApplyChoiceOrder(sut, [3, 0, 1, 2]);

        // Assert
        Assert.Equal(["Uranus", "Earth", "Saturn", "Mars"], sut.AnswerChoices);
        Assert.Equal([2, 0], sut.CorrectAnswerIndices);

        var evaluation = sut.Evaluate(new QuestionResponse
        {
            QuestionType = QuestionType.MultipleSelect,
            Response = new[] { 0, 2 },
            TimeTaken = TimeSpan.FromSeconds(1)
        });

        Assert.True(evaluation.Evaluation.IsT0);
        Assert.True(evaluation.Evaluation.AsT0);
    }

    [Fact]
    public void ShuffleQuestionChoices_WithSeededRandom_PreservesCorrectAnswersForSupportedQuestionTypes()
    {
        // Arrange
        var multipleChoiceQuestion = new MultipleChoiceQuestion
        {
            Text = "Which element has the chemical symbol O?",
            AnswerChoices = ["Oxygen", "Gold", "Silver", "Hydrogen"],
            CorrectAnswerIndex = 0
        };

        var multipleSelectQuestion = new MultipleSelectQuestion
        {
            Text = "Select all primary colors.",
            AnswerChoices = ["Red", "Green", "Blue", "Yellow"],
            CorrectAnswerIndices = [0, 2, 3]
        };

        var trueFalseQuestion = new TrueFalseQuestion
        {
            Text = "Water freezes at 0C.",
            CorrectAnswer = true
        };

        var questions = new Question[] { multipleChoiceQuestion, multipleSelectQuestion, trueFalseQuestion };

        // Act
        AnswerChoiceRandomizer.ShuffleQuestionChoices(questions, new Random(7));

        // Assert
        Assert.Equal(["Gold", "Hydrogen", "Oxygen", "Silver"], multipleChoiceQuestion.AnswerChoices.OrderBy(choice => choice));
        Assert.Equal("Oxygen", multipleChoiceQuestion.AnswerChoices[multipleChoiceQuestion.CorrectAnswerIndex]);

        Assert.Equal(["Blue", "Green", "Red", "Yellow"], multipleSelectQuestion.AnswerChoices.OrderBy(choice => choice));
        Assert.Equal(
            ["Blue", "Red", "Yellow"],
            multipleSelectQuestion.CorrectAnswerIndices
                .Select(index => multipleSelectQuestion.AnswerChoices[index])
                .OrderBy(choice => choice));

        Assert.True(trueFalseQuestion.CorrectAnswer);
    }
}