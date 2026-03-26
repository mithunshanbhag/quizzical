namespace Quizzical.Models;

public class QuizEvaluation
{
    public Dictionary<Question, QuestionEvaluation> Evaluations { get; } = [];
}