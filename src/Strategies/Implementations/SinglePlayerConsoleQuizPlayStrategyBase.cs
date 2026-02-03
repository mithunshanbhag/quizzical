namespace Quizzical.Cli.Strategies.Implementations;

/// <summary>
///     Base strategy class for console-based, single-player quizzes.
/// </summary>
public abstract class SinglePlayerConsoleQuizPlayStrategyBase : IQuizPlayStrategy
{
    /// <remarks>
    ///     This uses the template design pattern. It defines a skeleton (template) method that calls into
    ///     other virtual/abstract (hook) methods overridden by derived classes.
    /// </remarks>
    public async Task<QuizEvaluation> ExecuteAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        var quizResponse = new QuizEvaluation();

        var toContinue = true;

        for (var index = 0; index < quiz.Questions.Length && toContinue; index++)
        {
            var currentQuestion = quiz.Questions[index];

            DisplayQuestion(currentQuestion, index, quiz.Questions.Length);

            var selectedAnswer = CaptureUserResponse(currentQuestion);

            var evaluation = currentQuestion.Evaluate(selectedAnswer);

            quizResponse.Evaluations.Add(currentQuestion, evaluation);

            toContinue = ShowEvaluation(currentQuestion, evaluation, index, quiz.Questions.Length);
        }

        return await Task.FromResult(quizResponse);
    }

    protected virtual void DisplayQuestion(Question question, int index, int totalQuestions)
    {
        AnsiConsole.Clear();
        AnsiConsole.Progress()
            .Columns(new TaskDescriptionColumn(), new ProgressBarColumn())
            .Start(ctx =>
            {
                var progressTask = ctx.AddTask($"Question {index + 1} of {totalQuestions}", new ProgressTaskSettings { MaxValue = totalQuestions });
                progressTask.Increment(index);
            });

        AnsiConsole.MarkupLineInterpolated($"[yellow]{question.Text}[/]");
        AnsiConsole.WriteLine();
    }

    protected abstract QuestionResponse CaptureUserResponse(Question question);

    protected virtual bool ShowEvaluation(Question question, QuestionEvaluation evaluation, int index, int totalQuestions)
    {
        if (question.ExplanationText is not null)
        {
            AnsiConsole.Markup(evaluation.Evaluation.IsT0
                ? evaluation.Evaluation.AsT0
                    ? $"[green]Correct ({evaluation.TimeTaken.Seconds} secs): [/]"
                    : $"[red]Incorrect ({evaluation.TimeTaken.Seconds} secs): [/]"
                : $"[yellow]Skipped ({evaluation.TimeTaken.Seconds} secs): [/]");
            AnsiConsole.MarkupLine(question.ExplanationText);
        }

        AnsiConsole.WriteLine();
        var toContinue = AnsiConsole.Prompt(
            new ConfirmationPrompt("Continue?")
                .HideDefaultValue());

        return toContinue;
    }
}