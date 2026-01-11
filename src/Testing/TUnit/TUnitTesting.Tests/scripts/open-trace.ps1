<#
.SYNOPSIS
    Opens a Playwright trace file in the trace viewer.
    
.DESCRIPTION
    This script opens a Playwright trace file (.zip) in the Playwright trace viewer.
    It can accept either a trace filename or a full path to the trace file.
    
.PARAMETER TraceFile
    The trace file to open. Can be:
    - Just the filename (e.g., "TUnit.Core.TestDetails.NavigateToAuthenticatedPage.zip")
      The script will look for it in the traces directory
    - A full path to the trace file
    
.EXAMPLE
    .\open-trace.ps1 "TUnit.Core.TestDetails.NavigateToAuthenticatedPage.zip"
    
.EXAMPLE
    .\open-trace.ps1 "traces\TUnit.Core.TestDetails.NavigateToAuthenticatedPage.zip"
    
.EXAMPLE
    .\open-trace.ps1 "C:\Dev\Code\dotnet-samples\src\Testing\TUnit\TUnitTesting.Tests\traces\TUnit.Core.TestDetails.NavigateToAuthenticatedPage.zip"
#>

param(
    [Parameter(Mandatory=$false, Position=0)]
    [string]$TraceFile
)

$ErrorActionPreference = "Stop"

# Get the script directory (where this script is located) and project root (one level up)
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir

# Find the playwright.ps1 script - try net10.0 first, then net9.0, then net8.0
$TargetFrameworks = @("net10.0", "net9.0", "net8.0")
$PlaywrightScript = $null

foreach ($framework in $TargetFrameworks) {
    $PossiblePath = Join-Path $ProjectRoot "bin\Debug\$framework\playwright.ps1"
    if (Test-Path $PossiblePath) {
        $PlaywrightScript = $PossiblePath
        break
    }
}

if (-not $PlaywrightScript) {
    Write-Error "Could not find playwright.ps1 in bin\Debug\net10.0, net9.0, or net8.0. Please build the project first."
    exit 1
}

# If no trace file specified, prompt user to select one
if ([string]::IsNullOrWhiteSpace($TraceFile)) {
    $TracesDir = Join-Path $ProjectRoot "playwright\traces"
    
    if (-not (Test-Path $TracesDir)) {
        Write-Error "Trace directory not found: $TracesDir"
        exit 1
    }
    
    $Traces = @(Get-ChildItem -Path $TracesDir -Filter "*.zip" | Sort-Object LastWriteTime -Descending)
    
    if ($Traces.Count -eq 0) {
        Write-Host "No trace files found in $TracesDir" -ForegroundColor Yellow
        exit 0
    }
    
    Write-Host "`nAvailable trace files:" -ForegroundColor Cyan
    for ($i = 0; $i -lt $Traces.Count; $i++) {
        $trace = $Traces[$i]
        $number = $i + 1
        Write-Host "  $number. $($trace.Name) ($(Get-Date $trace.LastWriteTime -Format 'yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Green
    }
    
    $selection = Read-Host "`nSelect a trace file (1-$($Traces.Count))"
    
    if ([string]::IsNullOrWhiteSpace($selection)) {
        Write-Host "No selection made. Exiting." -ForegroundColor Yellow
        exit 0
    }
    
    $selectedIndex = 0
    if (-not [int]::TryParse($selection, [ref]$selectedIndex)) {
        Write-Error "Invalid selection: '$selection'. Please enter a number between 1 and $($Traces.Count)."
        exit 1
    }
    
    if ($selectedIndex -lt 1 -or $selectedIndex -gt $Traces.Count) {
        Write-Error "Selection out of range: $selectedIndex. Please enter a number between 1 and $($Traces.Count)."
        exit 1
    }
    
    $TraceFile = $Traces[$selectedIndex - 1].Name
    Write-Host "Selected: $TraceFile`n" -ForegroundColor Cyan
}

# Resolve the trace file path
$ResolvedTraceFile = $null

# Check if it's already a full path
if ([System.IO.Path]::IsPathRooted($TraceFile)) {
    if (Test-Path $TraceFile) {
        $ResolvedTraceFile = (Resolve-Path $TraceFile).Path
    }
} else {
    # Try as-is first (might be relative path like traces\...)
    $PossiblePath = Join-Path $ProjectRoot $TraceFile
    if (Test-Path $PossiblePath) {
        $ResolvedTraceFile = (Resolve-Path $PossiblePath).Path
    } else {
        # Try in the playwright-traces directory
        $PossiblePath = Join-Path $ProjectRoot "playwright\traces\$TraceFile"
        if (Test-Path $PossiblePath) {
            $ResolvedTraceFile = (Resolve-Path $PossiblePath).Path
        }
    }
}

if (-not $ResolvedTraceFile) {
    Write-Error "Trace file not found: $TraceFile"
    Write-Host "`nTried the following paths:" -ForegroundColor Yellow
    Write-Host "  - $([System.IO.Path]::Combine($ProjectRoot, $TraceFile))" -ForegroundColor Gray
    Write-Host "  - $([System.IO.Path]::Combine($ProjectRoot, 'playwright\traces', $TraceFile))" -ForegroundColor Gray
    exit 1
}

# Open the trace
Write-Host "Opening trace: $ResolvedTraceFile" -ForegroundColor Cyan
Write-Host "Using Playwright script: $PlaywrightScript`n" -ForegroundColor Gray

& $PlaywrightScript show-trace $ResolvedTraceFile

