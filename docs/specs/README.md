# Quizzical Specifications

## Product Summary

Quizzical is a .NET 10 console application that generates AI-assisted quizzes for a single player. The current experience focuses on a short terminal-based game loop where the player selects a quiz type, topic, number of questions, and difficulty level before answering generated questions.

## Supported Quiz Modes

| Quiz type       | Status                                | Notes                                                                                     |
| --------------- | ------------------------------------- | ----------------------------------------------------------------------------------------- |
| Multiple-choice | Supported                             | Presents several choices and evaluates one correct answer.                                |
| True/false      | Supported                             | Presents a boolean choice and evaluates the selected answer.                              |
| Groupable items | In code, not yet surfaced in the menu | The domain model and play strategy exist, but the current menu does not expose this mode. |
| Sequence        | Planned                               | Reserved for future work.                                                                 |
| Match           | Planned                               | Reserved for future work.                                                                 |

## Functional Requirements

1. Start the application from the console.
2. Display the Quizzical banner.
3. Prompt the player for quiz configuration when it is not supplied through configuration.
4. Generate quiz questions by calling the configured OpenAI chat model.
5. Play through each question in the terminal and evaluate the response.
6. Show a game-over summary with correct, incorrect, and skipped answers.
7. Ask the player whether they want to play again.

## Technical Notes

- Main project: `/src/Quizzical.csproj`
- Target framework: `net10.0`
- Local workflow script: `/run-local.ps1`
- Unit tests: `/tests/Quizzical.UnitTests`
- Root solution/workspace: `/Quizzical.slnx`

## Configuration

Quizzical expects OpenAI configuration via user secrets or another .NET configuration source:

- `OpenAi:ApiKey`
- `OpenAi:Model`

Optional quiz defaults can also be supplied through configuration under `QuizConfig`.
