[CmdletBinding()]
param(
    [ValidateSet('help', 'restore', 'build', 'run', 'app', 'test', 'tests', 'unit-tests', 'e2e-tests', 'format', 'all')]
    [string]$Task = 'help',

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$RemainingArgs
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionPath = Join-Path $repoRoot 'Quizzical.slnx'
$appProjectPath = Join-Path $repoRoot 'src/Quizzical.csproj'
$unitTestProjectPath = Join-Path $repoRoot 'tests/Quizzical.UnitTests/Quizzical.UnitTests.csproj'
$e2eTestProjectPath = Join-Path $repoRoot 'tests/Quizzical.E2ETests/Quizzical.E2ETests.csproj'

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

switch ($Task) {
    'help' {
        Write-Host 'Quizzical local workflow helper' -ForegroundColor Green
        Write-Host ''
        Write-Host 'Examples:'
        Write-Host '  ./run-local.ps1 restore'
        Write-Host '  ./run-local.ps1 build'
        Write-Host '  ./run-local.ps1 tests'
        Write-Host '  ./run-local.ps1 unit-tests'
        Write-Host '  ./run-local.ps1 e2e-tests'
        Write-Host '  ./run-local.ps1 format'
        Write-Host '  ./run-local.ps1 app'
        Write-Host '  ./run-local.ps1 all'
        break
    }
    'restore' {
        Invoke-DotNetCommand -Arguments @('restore', $solutionPath)
        break
    }
    'build' {
        Invoke-DotNetCommand -Arguments @('build', '--nologo', $solutionPath)
        break
    }
    'run' {
        $arguments = @('run', '--project', $appProjectPath)

        if ($null -ne $RemainingArgs -and $RemainingArgs.Length -gt 0) {
            $arguments += '--'
            $arguments += $RemainingArgs
        }

        Invoke-DotNetCommand -Arguments $arguments
        break
    }
    'app' {
        $arguments = @('run', '--project', $appProjectPath)

        if ($null -ne $RemainingArgs -and $RemainingArgs.Length -gt 0) {
            $arguments += '--'
            $arguments += $RemainingArgs
        }

        Invoke-DotNetCommand -Arguments $arguments
        break
    }
    'test' {
        Invoke-DotNetCommand -Arguments @('test', '--nologo', '-v', 'minimal', $solutionPath)
        break
    }
    'tests' {
        Invoke-DotNetCommand -Arguments @('test', '--nologo', '-v', 'minimal', $solutionPath)
        break
    }
    'unit-tests' {
        Invoke-DotNetCommand -Arguments @('test', '--nologo', '-v', 'minimal', $unitTestProjectPath)
        break
    }
    'e2e-tests' {
        Invoke-DotNetCommand -Arguments @('test', '--nologo', '-v', 'minimal', $e2eTestProjectPath)
        break
    }
    'format' {
        Invoke-DotNetCommand -Arguments @('format', '--verify-no-changes', $solutionPath)
        break
    }
    'all' {
        Invoke-DotNetCommand -Arguments @('restore', $solutionPath)
        Invoke-DotNetCommand -Arguments @('format', '--verify-no-changes', $solutionPath)
        Invoke-DotNetCommand -Arguments @('build', '--nologo', $solutionPath)
        Invoke-DotNetCommand -Arguments @('test', '--nologo', '-v', 'minimal', $solutionPath)
        break
    }
}
