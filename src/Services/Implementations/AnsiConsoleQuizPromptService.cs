namespace Quizzical.Services.Implementations;

public class AnsiConsoleQuizPromptService : IQuizPromptService
{
    public QuestionType PromptQuizType()
    {
        AnsiConsole.Clear();

        return AnsiConsole.Prompt(
            new SelectionPrompt<QuestionType>()
                .Title($"What type of quiz would you like to play? {Emoji.Known.GrinningFace}")
                .PageSize(8)
                .HighlightStyle(Color.Cyan1.ToString())
                .MoreChoicesText("[cyan](Move up and down to reveal more topics)[/]")
                .AddChoices(QuestionType.MultipleChoice, QuestionType.MultipleSelect, QuestionType.TrueFalse));
    }

    public string PromptTopic(IReadOnlyCollection<string> topics)
    {
        AnsiConsole.Clear();

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"What's your topic? {Emoji.Known.ThinkingFace}")
                .PageSize(8)
                .HighlightStyle(Color.Cyan1.ToString())
                .MoreChoicesText("[cyan](Move up and down to reveal more topics)[/]")
                .AddChoices(topics));
    }

    public int PromptNumberOfQuestions()
    {
        AnsiConsole.Clear();

        return AnsiConsole.Prompt(
            new TextPrompt<int>("How many questions would you like to answer?")
                .DefaultValue(QuizConstants.DefaultNumberOfQuestions)
                .Validate(answer => answer is < 1 or > 20
                    ? ValidationResult.Error("Please enter a number between 1 and 20.")
                    : ValidationResult.Success()));
    }

    public QuestionDifficultyLevel PromptDifficultyLevel()
    {
        AnsiConsole.Clear();

        return AnsiConsole.Prompt(
            new SelectionPrompt<QuestionDifficultyLevel>()
                .Title($"What difficulty level would you like to play? {Emoji.Known.LevelSlider}")
                .PageSize(8)
                .HighlightStyle(Color.Cyan1.ToString())
                .AddChoices(QuestionDifficultyLevel.Easy, QuestionDifficultyLevel.Medium, QuestionDifficultyLevel.Hard));
    }

    public string PromptSingleSelection(IReadOnlyCollection<string> choices)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(Color.Cyan1.ToString())
                .PageSize(10)
                .AddChoices(choices));
    }

    public string[] PromptMultiSelection(IReadOnlyCollection<string> choices)
    {
        return
        [
            ..AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .HighlightStyle(Color.Cyan1.ToString())
                    .PageSize(10)
                    .AddChoices(choices))
        ];
    }

    public bool PromptConfirmation(string prompt, bool hideDefaultValue = false)
    {
        var confirmationPrompt = new ConfirmationPrompt(prompt);

        if (hideDefaultValue)
            confirmationPrompt.HideDefaultValue();

        return AnsiConsole.Prompt(confirmationPrompt);
    }
}