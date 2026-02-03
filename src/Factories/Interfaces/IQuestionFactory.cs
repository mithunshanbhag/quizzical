namespace Quizzical.Cli.Factories.Interfaces;

public interface IQuestionFactory
{
    Task<Question[]> GenerateAsync(QuizConfig request, CancellationToken cancellationToken = default);
}