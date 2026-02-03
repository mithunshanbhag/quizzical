namespace Quizzical.Strategies.Interfaces;

public interface IQuizPlayStrategy
{
    Task<QuizEvaluation> ExecuteAsync(Quiz quiz, CancellationToken cancellationToken = default);
}