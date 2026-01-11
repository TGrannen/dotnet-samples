<#
.SYNOPSIS
    Runs the Playwright code generator to record tests interactively.
    
.DESCRIPTION
    This script runs the Playwright code generator (codegen) which opens a browser window
    for interaction and the Playwright Inspector for recording, copying, and managing
    your generated tests. It defaults to the Guillotine home URL and loads authentication
    state if available in the playwright/auth directory.
    
.PARAMETER Url
    Optional URL to open in the browser. Defaults to "https://guillotineleagues.com/" if not provided.
    
.EXAMPLE
    .\codegen.ps1
    
.EXAMPLE
    .\codegen.ps1 "https://demo.playwright.dev/todomvc"
    
.EXAMPLE
    .\codegen.ps1 "demo.playwright.dev/todomvc"
#>

param(
    [Parameter(Mandatory=$false, Position=0)]
    [string]$Url
)

$ErrorActionPreference = "Stop"

# Get the script directory (where this script is located) and project root (one level up)
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir

# Default URL to Guillotine home
if ([string]::IsNullOrWhiteSpace($Url)) {
    $Url = "https://guillotineleagues.com/"
}

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

# Check for auth state file
$AuthStatePath = Join-Path $ProjectRoot "playwright\auth\state.json"
$HasAuthState = Test-Path $AuthStatePath

# Build the command arguments
$Arguments = @("codegen")
if ($HasAuthState) {
    $ResolvedAuthStatePath = (Resolve-Path $AuthStatePath).Path
    $Arguments += "--load-storage"
    $Arguments += $ResolvedAuthStatePath
    Write-Host "Using authentication state: $ResolvedAuthStatePath" -ForegroundColor Green
}
$Arguments += $Url

# Run the code generator
Write-Host "Starting Playwright code generator..." -ForegroundColor Cyan
Write-Host "Opening URL: $Url" -ForegroundColor Cyan
if (-not $HasAuthState) {
    Write-Host "No authentication state found at: $AuthStatePath" -ForegroundColor Yellow
}
Write-Host "Using Playwright script: $PlaywrightScript`n" -ForegroundColor Gray

& $PlaywrightScript @Arguments

