[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('app', 'tests', 'unit-tests', 'e2e-tests')]
    [string]$Target = 'app',

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$RemainingArgs
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$appProjectPath = Join-Path $repoRoot 'src\Quizzical\Quizzical.csproj'
$testsRootPath = Join-Path $repoRoot 'tests'
$unitTestProjectPath = Join-Path $testsRootPath 'Quizzical.UnitTests\Quizzical.UnitTests.csproj'

function Invoke-DotNetCommand {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    Write-Host "dotnet $($Arguments -join ' ')" -ForegroundColor Cyan
    & dotnet @Arguments

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet command failed with exit code $LASTEXITCODE."
    }
}

function Resolve-ExistingFilePath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,

        [Parameter(Mandatory = $true)]
        [string]$Description
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "Could not find $Description at '$Path'."
    }

    return (Resolve-Path -LiteralPath $Path).Path
}

function Get-TestProjectPaths {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Pattern,

        [Parameter(Mandatory = $true)]
        [string]$Description
    )

    if (-not (Test-Path -LiteralPath $testsRootPath -PathType Container)) {
        throw "Could not find tests folder at '$testsRootPath'."
    }

    $projectPaths = @(
        Get-ChildItem -Path $testsRootPath -Recurse -Filter $Pattern -File |
            Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
            Sort-Object -Property FullName |
            ForEach-Object { $_.FullName }
    )

    if ($projectPaths.Count -eq 0) {
        throw "Could not find any $Description test projects in '$testsRootPath'. Only checked-in *.csproj files outside bin\ and obj\ are considered."
    }

    return $projectPaths
}

switch ($Target) {
    'app' {
        $resolvedAppProjectPath = Resolve-ExistingFilePath -Path $appProjectPath -Description 'the app project'
        $arguments = @('run', '--project', $resolvedAppProjectPath)

        if ($null -ne $RemainingArgs -and $RemainingArgs.Length -gt 0) {
            $arguments += '--'
            $arguments += $RemainingArgs
        }

        Invoke-DotNetCommand -Arguments $arguments
        break
    }
    'tests' {
        $testProjectPaths = Get-TestProjectPaths -Pattern '*Tests.csproj' -Description 'local'

        foreach ($testProjectPath in $testProjectPaths) {
            Invoke-DotNetCommand -Arguments @('test', '--nologo', '-v', 'minimal', $testProjectPath)
        }
        break
    }
    'unit-tests' {
        $resolvedUnitTestProjectPath = Resolve-ExistingFilePath -Path $unitTestProjectPath -Description 'the unit test project'
        Invoke-DotNetCommand -Arguments @('test', '--nologo', '-v', 'minimal', $resolvedUnitTestProjectPath)
        break
    }
    'e2e-tests' {
        $e2eTestProjectPaths = Get-TestProjectPaths -Pattern '*E2E*.csproj' -Description 'E2E'

        foreach ($e2eTestProjectPath in $e2eTestProjectPaths) {
            Invoke-DotNetCommand -Arguments @('test', '--nologo', '-v', 'minimal', $e2eTestProjectPath)
        }
        break
    }
}
