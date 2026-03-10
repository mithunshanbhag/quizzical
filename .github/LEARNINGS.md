# LEARNINGS

## 2026-03-10T13:04:34Z

- Quizzical is a .NET 10 console application rooted at `src\Quizzical.csproj`; the new repo workspace is `Quizzical.slnx`.
- Local developer workflows are centralized in `run-local.ps1`:
  - `.\run-local.ps1 build`
  - `.\run-local.ps1 test`
  - `.\run-local.ps1 unit-tests`
  - `.\run-local.ps1 format`
- Current automated tests live in `tests\Quizzical.UnitTests\Quizzical.UnitTests.csproj`.
- `QuizFactory.GenerateAsync(...)` now rejects unsupported `QuestionType` values before calling the question generator, and the unit tests assert that unsupported types do not invoke `IQuestionFactory`.
- Product/spec documentation for this CLI app now lives under `docs\specs\README.md` and `docs\specs\ui.md`.
