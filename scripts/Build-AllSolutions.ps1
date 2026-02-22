# Build all solutions under src/. Run from repo root.
# Exits with 0 if all build; non-zero if any fail.

$ErrorActionPreference = 'Stop'
$srcPath = Join-Path $PSScriptRoot '..' 'src'
if (-not (Test-Path $srcPath)) {
    Write-Error "src folder not found at $srcPath. Run from repo root or ensure scripts live in repo/scripts."
    exit 1
}

$slns = Get-ChildItem -Path $srcPath -Recurse -Filter '*.sln' -File | Sort-Object FullName
$failed = [System.Collections.Generic.List[string]]::new()

foreach ($sln in $slns) {
    $rel = $sln.FullName.Replace((Get-Location).Path + [IO.Path]::DirectorySeparatorChar, '')
    Write-Host "Building $rel ..."
    $proc = Start-Process -FilePath 'dotnet' -ArgumentList 'build', $sln.FullName, '-c', 'Release' -NoNewWindow -Wait -PassThru
    if ($proc.ExitCode -ne 0) {
        $failed.Add($rel)
        Write-Host "  FAILED (exit $($proc.ExitCode))"
    } else {
        Write-Host "  OK"
    }
}

if ($failed.Count -gt 0) {
    Write-Host ""
    Write-Host "Failed solutions ($($failed.Count)):"
    $failed | ForEach-Object { Write-Host "  - $_" }
    exit 1
}

Write-Host ""
Write-Host "All $($slns.Count) solutions built successfully."
exit 0
