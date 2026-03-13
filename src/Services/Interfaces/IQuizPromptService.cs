namespace Quizzical.Services.Interfaces;

public interface IQuizPromptService
{
    QuestionType PromptQuizType();

    string PromptTopic(IReadOnlyCollection<string> topics);

    int PromptNumberOfQuestions();

    QuestionDifficultyLevel PromptDifficultyLevel();

    string PromptSingleSelection(IReadOnlyCollection<string> choices);

    string[] PromptMultiSelection(IReadOnlyCollection<string> choices);

    bool PromptConfirmation(string prompt, bool hideDefaultValue = false);
}