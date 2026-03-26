namespace Quizzical.Misc.Utilities;

internal static class AnswerChoiceRandomizer
{
    internal static void ShuffleQuestionChoices(Question[] questions, Random random)
    {
        ArgumentNullException.ThrowIfNull(questions);
        ArgumentNullException.ThrowIfNull(random);

        foreach (var question in questions)
        {
            switch (question)
            {
                case MultipleChoiceQuestion { AnswerChoices.Length: > 1 } multipleChoiceQuestion:
                    ApplyChoiceOrder(
                        multipleChoiceQuestion,
                        CreateShuffledChoiceOrder(multipleChoiceQuestion.AnswerChoices.Length, random));
                    break;

                case MultipleSelectQuestion { AnswerChoices.Length: > 1 } multipleSelectQuestion:
                    ApplyChoiceOrder(
                        multipleSelectQuestion,
                        CreateShuffledChoiceOrder(multipleSelectQuestion.AnswerChoices.Length, random));
                    break;
            }
        }
    }

    internal static void ApplyChoiceOrder(MultipleChoiceQuestion question, IReadOnlyList<int> originalIndicesInNewOrder)
    {
        ArgumentNullException.ThrowIfNull(question);

        ValidateChoiceOrder(originalIndicesInNewOrder, question.AnswerChoices.Length);

        var originalChoices = question.AnswerChoices;
        question.AnswerChoices = originalIndicesInNewOrder
            .Select(index => originalChoices[index])
            .ToArray();
        question.CorrectAnswerIndex = FindRemappedIndex(originalIndicesInNewOrder, question.CorrectAnswerIndex);
    }

    internal static void ApplyChoiceOrder(MultipleSelectQuestion question, IReadOnlyList<int> originalIndicesInNewOrder)
    {
        ArgumentNullException.ThrowIfNull(question);

        ValidateChoiceOrder(originalIndicesInNewOrder, question.AnswerChoices.Length);

        var originalChoices = question.AnswerChoices;
        question.AnswerChoices = originalIndicesInNewOrder
            .Select(index => originalChoices[index])
            .ToArray();
        question.CorrectAnswerIndices = question.CorrectAnswerIndices
            .Select(index => FindRemappedIndex(originalIndicesInNewOrder, index))
            .ToArray();
    }

    private static int[] CreateShuffledChoiceOrder(int answerChoiceCount, Random random)
    {
        var shuffledIndices = Enumerable.Range(0, answerChoiceCount).ToArray();

        for (var index = shuffledIndices.Length - 1; index > 0; index--)
        {
            var swapIndex = random.Next(index + 1);
            (shuffledIndices[index], shuffledIndices[swapIndex]) = (shuffledIndices[swapIndex], shuffledIndices[index]);
        }

        return shuffledIndices;
    }

    private static int FindRemappedIndex(IReadOnlyList<int> originalIndicesInNewOrder, int originalIndex)
    {
        for (var index = 0; index < originalIndicesInNewOrder.Count; index++)
        {
            if (originalIndicesInNewOrder[index] == originalIndex)
                return index;
        }

        throw new ArgumentException($"Original index {originalIndex} was not found in the supplied choice order.");
    }

    private static void ValidateChoiceOrder(IReadOnlyList<int> originalIndicesInNewOrder, int answerChoiceCount)
    {
        ArgumentNullException.ThrowIfNull(originalIndicesInNewOrder);

        var expectedIndices = Enumerable.Range(0, answerChoiceCount).ToArray();

        if (originalIndicesInNewOrder.Count != answerChoiceCount ||
            !originalIndicesInNewOrder.OrderBy(index => index).SequenceEqual(expectedIndices))
            throw new ArgumentException("The supplied choice order must be a permutation of the original answer choice indices.");
    }
}