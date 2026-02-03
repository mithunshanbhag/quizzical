namespace Quizzical.Cli.Factories.Implementations;

public class QuizFactory(IQuestionFactory questionFactory, ILogger<QuizFactory> logger) : IQuizFactory
{
    public async Task<Quiz> GenerateAsync(QuizConfig request, CancellationToken cancellationToken = default)
    {
        var questions = await questionFactory.GenerateAsync(request, cancellationToken);

        return request.QuestionType switch
        {
            QuestionType.MultipleChoice or QuestionType.TrueFalse or QuestionType.GroupableItems => new Quiz { Config = request, Questions = questions },
            _ => throw new NotSupportedException($"Question type {request.QuestionType} is not supported yet.")
        };
    }
}