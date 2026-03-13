namespace Quizzical.Factories.Implementations;

public class ConfiguredQuestionFactory(IConfiguration configuration) : IQuestionFactory
{
    public Task<Question[]> GenerateAsync(QuizConfig request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Question[] questions = request.QuestionType switch
        {
            QuestionType.MultipleChoice => BindQuestions<MultipleChoiceQuestion>(request),
            QuestionType.MultipleSelect => BindQuestions<MultipleSelectQuestion>(request),
            QuestionType.TrueFalse => BindQuestions<TrueFalseQuestion>(request),
            QuestionType.GroupableItems => BindQuestions<GroupableItemsQuestion>(request),
            _ => throw new NotSupportedException($"Question type {request.QuestionType} is not supported yet.")
        };

        return Task.FromResult(questions);
    }

    private TQuestion[] BindQuestions<TQuestion>(QuizConfig request)
        where TQuestion : Question
    {
        var configuredQuestions = configuration
            .GetSection($"{ConfigKeys.TestQuestionData}:{request.QuestionType}")
            .Get<TQuestion[]>();

        if (configuredQuestions is null || configuredQuestions.Length == 0)
            throw new InvalidOperationException($"No configured test questions were found for question type {request.QuestionType}.");

        if (request.NumberOfQuestions > configuredQuestions.Length)
        {
            throw new InvalidOperationException(
                $"Requested {request.NumberOfQuestions} configured test questions for {request.QuestionType}, but only {configuredQuestions.Length} were supplied.");
        }

        foreach (var question in configuredQuestions.Where(question => !question.QuestionTimeLimitInSecs.HasValue))
            question.QuestionTimeLimitInSecs = request.QuestionTimeLimitInSecs;

        return configuredQuestions
            .Take(request.NumberOfQuestions)
            .ToArray();
    }
}