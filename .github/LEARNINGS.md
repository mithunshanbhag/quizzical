# LEARNINGS

## 2026-02-17T09:06:00Z

- Project uses .NET 10; the main app is at `src\FinSkew.Ui\FinSkew.Ui.csproj` (Blazor WebAssembly).
- Run locally with `dotnet run --project .\src\FinSkew.Ui\FinSkew.Ui.csproj` or `.\run-local.ps1`.
- Run all tests with `dotnet test .\FinSkew.slnx` or `.\run-local.ps1 tests`.
  - Run only unit tests: `dotnet test .\tests\FinSkew.Ui.UnitTests\FinSkew.Ui.UnitTests.csproj` or `.\run-local.ps1 unit-tests`.
  - Run only E2E tests: `dotnet test .\tests\FinSkew.Ui.E2ETests\FinSkew.Ui.E2ETests.csproj` or `.\run-local.ps1 e2e-tests`.
- Current automated test projects:
  - `tests\FinSkew.Ui.UnitTests\FinSkew.Ui.UnitTests.csproj`
  - `tests\FinSkew.Ui.E2ETests\FinSkew.Ui.E2ETests.csproj`

## 2026-02-17T09:45:43Z

- Adding a new calculator in `FinSkew.Ui` follows a repeatable wiring pattern:
  - add route key in `src\FinSkew.Ui\Constants\RouteConstants.cs`
  - add page in `src\FinSkew.Ui\Components\Pages\`
  - register calculator service in `src\FinSkew.Ui\Misc\ExtensionMethods\WebAssemblyHostBuilderExtensions.cs`
  - add navigation link in `src\FinSkew.Ui\Components\Shared\NavMenu.razor`
- Validation commands used for this feature:
  - `dotnet build --nologo`
  - `dotnet test .\tests\FinSkew.Ui.UnitTests\FinSkew.Ui.UnitTests.csproj --nologo -v minimal`
  - `dotnet test .\tests\FinSkew.Ui.E2ETests\FinSkew.Ui.E2ETests.csproj --nologo -v minimal`

## 2026-02-19T06:16:11Z

- CAGR terminology should stay as "Invested Amount" (not "Initial Investment") across UI labels and tests to keep specs and assertions aligned.

## 2026-02-19T07:24:00Z

- Compound Interest, Simple Interest, and Lumpsum specs/UI/tests now use aligned result terminology: "Invested Amount", "Total Gain", and "Final Amount" (including chart labels and aria labels), so future updates should keep these strings consistent across docs, components, and E2E selectors.

## 2026-02-19T08:10:00Z

- SIP/Step-Up SIP and SCSS specs now consistently use `P` as "Invested Amount" and `A` as "Final Amount".

## 2026-02-19T09:05:00Z

- In XIRR, "Invested Amount" is the sum of all monthly outflow cashflows across the tenure (not a one-time principal), and this is now asserted in unit tests.
- XIRR terminology now standardizes on "Investment End Date" (not "Investment Maturity Date") across specs, UI labels/aria labels, and E2E selectors.

## 2026-02-19T10:00:00Z

- EMI terminology is now aligned across spec/UI/E2E on "Loan Amount", "Monthly EMI", "Total Amount", and "Total Interest" (including chart labels/selectors), so future updates should keep these strings consistent.
- EMI computation keeps an explicit 0% annual-interest path (`EMI = principal / installments`) and tests assert `Total Amount = EMI × installments` and `Total Interest = Total Amount - Loan Amount`.

## 2026-02-19T10:20:00Z

- SCSS keeps "Annual Interest Rate" (7.4) and "Time Period (Years)" (5) as read-only input fields in the UI, and E2E tests assert both displayed values and `readonly` attributes to prevent regressions.

## 2026-02-24T11:36:33Z

- FinSkew.Ui now uses FluentValidation for calculator inputs: validators are auto-registered from the UI assembly in `WebAssemblyHostBuilderExtensions` via `AssemblyScanner.FindValidatorsInAssembly(...)`.
- All calculator services inherit shared validation from `CalculatorBase<TInput, TResult>` and call `ValidateInput(input)` before computation; invalid inputs now surface as `FluentValidation.ValidationException` instead of silent fallback results.

## 2026-02-26T07:24:05Z

- Post Office MIS summary panel now includes "Monthly Income" derived from total gain using `M = I / (N * 12)`, and MIS unit/E2E tests assert the new value/label.

## 2026-03-04T14:35:54+05:30

- `src\FinSkew.Ui\wwwroot\staticwebapp.config.json` in this repo is used for edge/runtime HTTP concerns (security headers and route-level cache behavior), not for calculator input-validation UX or calculation logic fixes.

## 2026-03-10T12:05:00Z

- `tests\FinSkew.Ui.E2ETests\WebServerFixture.cs` now starts a dedicated Blazor WASM dev server per suite on an isolated `127.0.0.1` port and disables launch profiles/browser auto-launch (`--no-launch-profile`) to avoid shared-port flakes.
- `tests\FinSkew.Ui.E2ETests\PlaywrightFixture.cs` must read `WebServerFixture.BaseUrl` and call `EnsureServerIsRunningAsync()` before each test instead of assuming `http://localhost:5000`.

## 2026-03-10T14:00:00Z

- Workspace-level Copilot hook for automated cleanup lives at `.github\hooks\cleanupcode-on-stop.json` and uses `.github\hooks\scripts\run-resharper-cleanup.ps1` to invoke `jb cleanupcode` against all `*.slnx` files from the repo root.
- The hook is configured on the `Stop` event, so cleanup runs when an agent session ends rather than during intermediate tool calls.

## 2026-03-10T09:01:41Z

- The unimplemented STP calculator is intentionally hidden for now: it has been removed from the nav menu, landing-page calculator cards, route constants, and current calculator lists in `README.md` and `docs\specs\README.md`, and the blank `STPCalculator.razor` page was removed so direct `/stp*` routes no longer resolve.
