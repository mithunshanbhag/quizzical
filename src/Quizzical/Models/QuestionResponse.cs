namespace Quizzical.Models;

public class QuestionResponse
{
    public QuestionType QuestionType { get; set; }

    public OneOf<int[], int, bool, None> Response { get; init; }

    public TimeSpan TimeTaken { get; init; }
}