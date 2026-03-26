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

## 2026-03-10T14:53:36Z

- `run-local.ps1 run` must guard `$RemainingArgs` for `$null` because the script runs with `Set-StrictMode -Version Latest`.
- The `run` task should invoke `dotnet run --project .\src\Quizzical.csproj` without `--no-build` so the convenience workflow works from a clean checkout/build state.

## 2026-03-13T15:29:09Z

- `tests\Quizzical.UnitTests` now uses xUnit.net v3 packages and `Assert.*` APIs instead of `FluentAssertions`.
- xUnit.net v3 analyzers expect async test calls that accept a `CancellationToken` to use `TestContext.Current.CancellationToken`.

## 2026-03-13T18:42:03Z

- `QuestionType.MultipleSelect` is now a supported quiz mode with a dedicated `MultipleSelectQuestion` model and `MultiSelectQuizPlayStrategy`.
- Multi-select evaluation uses exact-match, order-insensitive scoring against `CorrectAnswerIndices`, and the console flow keeps skip behavior explicit via `{SKIP}`.
- `QuestionFactory` includes extra prompt guidance for multi-select generation so AI responses populate `CorrectAnswerIndices` with multiple correct options.

## 2026-03-13T19:25:14Z

- `QuestionFactory` now shuffles `AnswerChoices` for `MultipleChoiceQuestion` and `MultipleSelectQuestion` immediately after deserialization, then remaps the stored correct index or indices before the quiz is played.
- The choice-randomization logic lives in `src\Misc\Utilities\AnswerChoiceRandomizer.cs` and is covered by unit tests via `InternalsVisibleTo("Quizzical.UnitTests")`.

## 2026-03-26T08:54:20Z

- The checked-in app project path is `src\Quizzical\Quizzical.csproj`; older references to `src\Quizzical.csproj` are stale.
- `run-local.ps1` now uses the skill-aligned targets `app`, `tests`, `unit-tests`, and `e2e-tests`.
- The `tests` target discovers checked-in `*Tests.csproj` projects under `tests\`, excluding `bin\` and `obj\`.
- The `e2e-tests` target fails fast with a clear error when no checked-in `*E2E*.csproj` project exists under `tests\`.
