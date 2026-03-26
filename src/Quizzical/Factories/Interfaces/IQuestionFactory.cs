namespace Quizzical.Factories.Interfaces;

public interface IQuestionFactory
{
    Task<Question[]> GenerateAsync(QuizConfig request, CancellationToken cancellationToken = default);
}