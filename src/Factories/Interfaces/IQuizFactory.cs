namespace Quizzical.Cli.Factories.Interfaces;

public interface IQuizFactory
{
    Task<Quiz> GenerateAsync(QuizConfig request, CancellationToken cancellationToken = default);
}