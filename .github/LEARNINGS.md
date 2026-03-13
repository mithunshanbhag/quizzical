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
