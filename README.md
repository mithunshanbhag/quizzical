# ✨ Quizzical: AI-generated quizzes! ✨

![Build status](https://img.shields.io/badge/build-manual-lightgrey?style=flat-square)
![Unit tests](https://img.shields.io/badge/tests-xUnit-blue?style=flat-square)

![quizzical](https://github.com/user-attachments/assets/a9a8db3e-5613-4f03-a649-9b0608edee48)

Quizzical is a .NET 10 console app for generating AI-assisted quizzes with OpenAI and Semantic Kernel-style chat completion workflows. It is currently focused on a single-player terminal experience powered by Spectre.Console.

## ✅ Current quiz support

| Type | Description |
| ---- | ----------- |
| ✔ `Multiple-choice` | Questions with multiple answer choices and one correct answer. |
| ✔ `Multi-select` | Questions with multiple answer choices and more than one correct answer. |
| ✔ `True/false` | Questions with a true or false answer. |
| ⚠ `Groupable items` | Implemented in the domain/strategy layer, but not yet exposed in the main menu. |
| ❌ `Sequence` | Answer involves arranging items in the correct sequence (for example chronology or ordered steps). |
| ❌ `Match` | Answer involves matching items to one another. |

> This app is primarily a playground for experimenting with OpenAI-powered quiz generation in a small CLI application.

## 📦 Installation

1. Install the .NET 10 SDK.
2. Clone the repository.
3. Restore dependencies:

   ```powershell
   dotnet restore .\Quizzical.slnx
   ```

4. Configure user secrets for the required OpenAI settings:

   ```powershell
   dotnet user-secrets --project .\src\Quizzical\Quizzical.csproj set "OpenAi:ApiKey" "<your-api-key>"
   dotnet user-secrets --project .\src\Quizzical\Quizzical.csproj set "OpenAi:Model" "<your-model-name>"
   ```

## 🚀 Usage

Run the app locally:

```powershell
dotnet run --project .\src\Quizzical\Quizzical.csproj
```

Or use the convenience script:

```powershell
.\run-local.ps1 app
```

The app will:

1. show the Quizzical banner,
2. ask you to choose a quiz type, topic, number of questions, and difficulty,
3. generate questions through the configured OpenAI model, and
4. guide you through the quiz in the terminal.

## 🛠 Build and run locally

Common commands:

```powershell
.\run-local.ps1 app
.\run-local.ps1 tests
.\run-local.ps1 unit-tests
.\run-local.ps1 e2e-tests
```

The script targets:

- app project: `.\src\Quizzical\Quizzical.csproj`
- all test projects under `.\tests\**\*Tests.csproj`
- unit tests: `.\tests\Quizzical.UnitTests\Quizzical.UnitTests.csproj`
- E2E tests: auto-discovered from `.\tests\**\*E2E*.csproj`

## 🧪 Run the tests

Run all tests in the workspace:

```powershell
.\run-local.ps1 tests
```

Run only unit tests:

```powershell
.\run-local.ps1 unit-tests
```

Run only E2E tests:

```powershell
.\run-local.ps1 e2e-tests
```

## 📚 Additional documentation

- Specs overview: [`docs/specs/README.md`](./docs/specs/README.md)
- Console interaction notes: [`docs/specs/ui.md`](./docs/specs/ui.md)
