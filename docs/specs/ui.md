# Console Interaction Specification

Quizzical currently has no graphical UI. Its user experience is a terminal-driven flow implemented with Spectre.Console.

## Console Flow

1. Render the `Quizzical` banner.
2. Ask the player to choose:
   - Quiz type
   - Topic
   - Number of questions
   - Difficulty level
3. Generate questions asynchronously and show a loading indicator.
4. Present each question and collect an answer in the terminal.
5. Show per-question evaluation feedback where explanation text is available.
6. Show the end-of-game breakdown chart and replay prompt.

## Interaction Constraints

- The current menu exposes multiple-choice, multi-select, and true/false quizzes.
- Multi-select questions use a terminal multi-selection prompt and require an exact match to the expected set of answers.
- The application requires valid OpenAI credentials to generate questions.
- The application is intentionally interactive; automated verification should focus on unit tests around domain and factory behavior.
