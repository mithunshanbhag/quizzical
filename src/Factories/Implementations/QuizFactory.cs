namespace Quizzical.Factories.Implementations;

public class QuizFactory(IQuestionFactory questionFactory) : IQuizFactory
{
    public async Task<Quiz> GenerateAsync(QuizConfig request, CancellationToken cancellationToken = default)
    {
        if (request.QuestionType is not (QuestionType.MultipleChoice or QuestionType.TrueFalse or QuestionType.GroupableItems))
            throw new NotSupportedException($"Question type {request.QuestionType} is not supported yet.");

        var questions = await questionFactory.GenerateAsync(request, cancellationToken);

        return new Quiz
        {
            Config = request,
            Questions = questions
        };
    }
}