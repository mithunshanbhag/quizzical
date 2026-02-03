namespace Quizzical.Models;

public class QuestionEvaluation
{
    public OneOf<bool, None> Evaluation { get; init; }

    public TimeSpan TimeTaken { get; init; }
}