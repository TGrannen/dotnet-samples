<#
.SYNOPSIS
Continuously queries the ACA /version endpoint once per second until stopped.

.EXAMPLE
.\poll-version.ps1 -BaseUrl (pulumi stack output stableUrl)

.EXAMPLE
.\poll-version.ps1 -Url "https://myapp.example.com/version"
#>

[CmdletBinding()]
param(
    # Base URL of the app, e.g. https://<app>.<env>.azurecontainerapps.io
    [Parameter(Mandatory = $false)]
    [string] $BaseUrl,

    # Full URL to the version endpoint, e.g. https://.../version
    [Parameter(Mandatory = $false)]
    [string] $Url,

    # Poll interval in seconds.
    [Parameter(Mandatory = $false)]
    [ValidateRange(0.1, 3600)]
    [double] $IntervalSeconds = 1,

    # Optional: create this file to stop the loop without Ctrl+C.
    [Parameter(Mandatory = $false)]
    [string] $StopFile
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Normalize-Url([string] $inputUrl) {
    $u = $inputUrl.Trim()
    if ($u.EndsWith('/')) { $u = $u.TrimEnd('/') }
    return $u
}

if ([string]::IsNullOrWhiteSpace($Url)) {
    if ([string]::IsNullOrWhiteSpace($BaseUrl)) {
        throw "Provide -Url or -BaseUrl."
    }
    $Url = "$(Normalize-Url $BaseUrl)/version"
}
else {
    $Url = Normalize-Url $Url
}

Write-Host "Polling: $Url"
Write-Host "Interval: $IntervalSeconds second(s)"
$stopHint = "Stop: Ctrl+C"
if (-not [string]::IsNullOrWhiteSpace($StopFile)) {
    $stopHint = "$stopHint or create file '$StopFile'"
}
Write-Host $stopHint
Write-Host ""

try {
    while ($true) {
        if (-not [string]::IsNullOrWhiteSpace($StopFile) -and (Test-Path -LiteralPath $StopFile)) {
            Write-Host "Stop file detected. Exiting."
            break
        }

        $ts = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff"
        try {
            # -UseBasicParsing is not needed in PowerShell 7+. Not supported in Windows PowerShell 5.1 with Invoke-RestMethod behavior changes,
            # so we avoid it for compatibility.
            $resp = Invoke-WebRequest -Uri $Url -Method GET -TimeoutSec 10

            $status = $resp.StatusCode
            $body = $resp.Content
            if ($null -eq $body) { $body = "" }
            $body = $body.Trim()
            if ([string]::IsNullOrWhiteSpace($body)) { $body = "<empty>" }

            Write-Host "[$ts] $status $body"
        }
        catch {
            $msg = $_.Exception.Message
            Write-Host "[$ts] ERROR $msg"
        }

        Start-Sleep -Milliseconds ([int][Math]::Round($IntervalSeconds * 1000))
    }
}
catch [System.Management.Automation.PipelineStoppedException] {
    # Ctrl+C commonly triggers pipeline stop in some hosts.
}
catch {
    throw
}
