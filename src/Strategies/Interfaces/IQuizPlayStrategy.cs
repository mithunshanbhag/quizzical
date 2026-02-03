namespace Quizzical.Cli.Strategies.Interfaces;

public interface IQuizPlayStrategy
{
    Task<QuizEvaluation> ExecuteAsync(Quiz quiz, CancellationToken cancellationToken = default);
}