using Quizzical.Misc.Utilities;

namespace Quizzical.Factories.Implementations;

public class QuestionFactory(ILogger<QuestionFactory> logger, ChatClient chatClient) : IQuestionFactory
{
    public async Task<Question[]> GenerateAsync(QuizConfig request, CancellationToken cancellationToken = default)
    {
        try
        {
            Question[] questions = request.QuestionType switch
            {
                QuestionType.MultipleChoice => await GenerateQuestionsAsync<MultipleChoiceQuestion>(request, cancellationToken),
                QuestionType.MultipleSelect => await GenerateQuestionsAsync<MultipleSelectQuestion>(request, cancellationToken),
                QuestionType.TrueFalse => await GenerateQuestionsAsync<TrueFalseQuestion>(request, cancellationToken),
                QuestionType.GroupableItems => await GenerateQuestionsAsync<GroupableItemsQuestion>(request, cancellationToken),
                _ => throw new NotImplementedException()
            };

            return questions;
        }
        catch (JsonException jse)
        {
            // @TODO
            Console.WriteLine(jse);
            throw;
        }
        catch (Exception e)
        {
            // @TODO
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<TQuestion[]> GenerateQuestionsAsync<TQuestion>(QuizConfig request, CancellationToken cancellationToken)
        where TQuestion : Question
    {
        var chatMessages = ComposeChatMessage(request);
        var completionOptions = ComposeChatCompletionOptions<TQuestion>();

        var result = await chatClient.CompleteChatAsync(chatMessages, completionOptions, cancellationToken);
        result.Value.Dump(logger); // dump debug details to console

        var questionList = JsonSerializer.Deserialize<QuestionList<TQuestion>>(result.Value.Content.FirstOrDefault()?.Text!);

        if (questionList.Questions is null)
            throw new JsonException("OpenAI returned an invalid question payload.");

        AnswerChoiceRandomizer.ShuffleQuestionChoices(questionList.Questions, Random.Shared);

        return questionList.Questions;
    }

    private static List<ChatMessage> ComposeChatMessage(QuizConfig request)
    {
        return
        [
            ChatMessage.CreateAssistantMessage(ChatConstants.AssistantMessage),
            ChatMessage.CreateUserMessage(ComposeQuestionRequest(request))
        ];
    }

    private static string ComposeQuestionRequest(QuizConfig request)
    {
        var questionTypeInstruction = request.QuestionType switch
        {
            QuestionType.MultipleChoice =>
                "multiple-choice questions with a single correct answer. Do not always place the correct answer first in AnswerChoices",
            QuestionType.MultipleSelect =>
                "multi-select questions with multiple correct answers. Each question must populate CorrectAnswerIndices with every correct option index, include at least two correct answers, and avoid clustering the correct answers at the start of AnswerChoices",
            _ => $"{request.QuestionType} questions"
        };

        return $"Create {request.NumberOfQuestions} {questionTypeInstruction} on the topic of {request.Topic} with difficulty level {request.DifficultyLevel}.";
    }

    private static ChatCompletionOptions ComposeChatCompletionOptions<TQuestion>() where TQuestion : Question
    {
        var jsonSerializerOptions = JsonSerializerOptions.Default;
        var jsonSchema = jsonSerializerOptions.GetJsonSchemaAsNode(typeof(QuestionList<TQuestion>));

        return new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "ExampleJson", BinaryData.FromString(jsonSchema.ToString()), jsonSchemaIsStrict: false)
        };
    }

    /// <summary>
    ///     A list of questions.
    /// </summary>
    /// <remarks>
    ///     This wrapper type is required to deserialize OpenAI's JSON response from OpenAI into a structured response.
    ///     See: https://community.openai.com/t/support-top-level-array-in-json-schema/896048
    /// </remarks>
    /// <typeparam name="TQuestion">
    ///     The type of question.
    /// </typeparam>
    private readonly struct QuestionList<TQuestion> where TQuestion : Question
    {
        public TQuestion[] Questions { get; init; }
    }
}