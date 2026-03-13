namespace Quizzical.Services.Implementations;

public class ConfiguredQuizPromptService : IQuizPromptService
{
    private readonly Queue<string> _singleSelectionResponses;
    private readonly Queue<string[]> _multiSelectionResponses;
    private readonly Queue<bool> _confirmationResponses;
    private readonly TestAutomationConfig _configuration;

    public ConfiguredQuizPromptService(IConfiguration configuration)
    {
        _configuration = configuration.GetSection(ConfigKeys.TestAutomation).Get<TestAutomationConfig>()
                         ?? throw new InvalidOperationException("Configured quiz prompt service requires TestAutomation settings.");

        _singleSelectionResponses = new Queue<string>(_configuration.SingleSelectionResponses);
        _multiSelectionResponses = new Queue<string[]>(_configuration.MultiSelectionResponses);
        _confirmationResponses = new Queue<bool>(_configuration.ConfirmationResponses);
    }

    public QuestionType PromptQuizType()
    {
        var questionType = _configuration.QuestionType
                           ?? throw new InvalidOperationException("TestAutomation:QuestionType must be configured when quiz type is not provided elsewhere.");

        AnsiConsole.WriteLine(questionType.ToString());
        return questionType;
    }

    public string PromptTopic(IReadOnlyCollection<string> topics)
    {
        var topic = EnsureChoice(
            _configuration.Topic,
            topics,
            "TestAutomation:Topic must be configured when topic is not provided elsewhere.");

        AnsiConsole.WriteLine(topic);
        return topic;
    }

    public int PromptNumberOfQuestions()
    {
        var numberOfQuestions = _configuration.NumberOfQuestions
                                ?? throw new InvalidOperationException(
                                    "TestAutomation:NumberOfQuestions must be configured when the quiz question count is not provided elsewhere.");

        AnsiConsole.WriteLine(numberOfQuestions.ToString());
        return numberOfQuestions;
    }

    public QuestionDifficultyLevel PromptDifficultyLevel()
    {
        var difficultyLevel = _configuration.DifficultyLevel
                              ?? throw new InvalidOperationException(
                                  "TestAutomation:DifficultyLevel must be configured when difficulty is not provided elsewhere.");

        AnsiConsole.WriteLine(difficultyLevel.ToString());
        return difficultyLevel;
    }

    public string PromptSingleSelection(IReadOnlyCollection<string> choices)
    {
        if (!_singleSelectionResponses.TryDequeue(out var response))
            throw new InvalidOperationException("No TestAutomation single-selection responses remain.");

        var selection = EnsureChoice(response, choices, $"Configured single-selection response '{response}' was not found in the available choices.");
        AnsiConsole.WriteLine(selection);
        return selection;
    }

    public string[] PromptMultiSelection(IReadOnlyCollection<string> choices)
    {
        if (!_multiSelectionResponses.TryDequeue(out var response))
            throw new InvalidOperationException("No TestAutomation multi-selection responses remain.");

        foreach (var selection in response)
            EnsureChoice(selection, choices, $"Configured multi-selection response '{selection}' was not found in the available choices.");

        AnsiConsole.WriteLine(string.Join(", ", response));
        return response;
    }

    public bool PromptConfirmation(string prompt, bool hideDefaultValue = false)
    {
        if (!_confirmationResponses.TryDequeue(out var response))
            throw new InvalidOperationException($"No TestAutomation confirmation responses remain for prompt '{prompt}'.");

        AnsiConsole.WriteLine(prompt);
        AnsiConsole.WriteLine(response ? bool.TrueString : bool.FalseString);
        return response;
    }

    private static string EnsureChoice(string? configuredChoice, IReadOnlyCollection<string> choices, string missingValueMessage)
    {
        if (string.IsNullOrWhiteSpace(configuredChoice))
            throw new InvalidOperationException(missingValueMessage);

        if (!choices.Contains(configuredChoice, StringComparer.Ordinal))
            throw new InvalidOperationException($"Configured choice '{configuredChoice}' was not found in the available choices.");

        return configuredChoice;
    }
}