# Learnings

- End-to-end CLI coverage lives in `tests\Quizzical.E2ETests` and can be run with `.\run-local.ps1 e2e-tests`.
- The app supports deterministic test runs via `Quizzical__TestConfigurationPath`, which points to a JSON file containing `QuizConfig`, `TestQuestionData`, and optional `TestAutomation` responses.
- Headless CLI test runs avoid Spectre console clearing/progress behavior when stdin/stdout are redirected, which keeps `CliWrap`-driven tests CI-friendly.
