<#
.SYNOPSIS
    Bumps the application version across all source files.

.DESCRIPTION
    Updates the version in CreAppConfig.cs, AssemblyInfo.cs, and ChaosRecipeEnhancer.UI.csproj.
    Run this on your release branch before merging to develop.

.PARAMETER Version
    The new version in X.Y.Z format (e.g., 3.28.1000).

.EXAMPLE
    .\scripts\bump-version.ps1 3.28.1000
    .\scripts\bump-version.ps1 -Version 3.28.1000
#>

param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

# ── Validate version format ─────────────────────────────────────────
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Host "ERROR: Version must be in X.Y.Z format (e.g., 3.28.1000)" -ForegroundColor Red
    Write-Host "  Got: $Version" -ForegroundColor Red
    exit 1
}

$parts = $Version.Split('.')
$majorMinor = "$($parts[0]).$($parts[1])"
$patch = [int]$parts[2]
$fourPart = "$Version.0"

# ApplicationRevision formula derived from existing convention:
#   v3.24.1000 -> Revision 10   (floor(1000/1000)*10 + 1000%1000 = 10)
#   v3.24.1001 -> Revision 11   (floor(1001/1000)*10 + 1001%1000 = 11)
#   v3.24.2000 -> Revision 20   (floor(2000/1000)*10 + 2000%1000 = 20)
$appRevision = [Math]::Floor($patch / 1000) * 10 + ($patch % 1000)

# ── Resolve paths relative to repo root ──────────────────────────────
$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
if (-not (Test-Path (Join-Path $repoRoot ".git"))) {
    # Fallback: script might be run from repo root directly
    $repoRoot = $PSScriptRoot | Split-Path -Parent
}
if (-not (Test-Path (Join-Path $repoRoot ".git"))) {
    $repoRoot = Get-Location
}

$appConfigPath = Join-Path $repoRoot "src/App/ChaosRecipeEnhancer.UI/Models/Config/CreAppConfig.cs"
$assemblyInfoPath = Join-Path $repoRoot "src/App/ChaosRecipeEnhancer.UI/Properties/AssemblyInfo.cs"
$csprojPath = Join-Path $repoRoot "src/App/ChaosRecipeEnhancer.UI/ChaosRecipeEnhancer.UI.csproj"

# Verify all files exist
$files = @($appConfigPath, $assemblyInfoPath, $csprojPath)
foreach ($file in $files) {
    if (-not (Test-Path $file)) {
        Write-Host "ERROR: File not found: $file" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "Bumping version to $Version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# ── CreAppConfig.cs ──────────────────────────────────────────────────
$content = Get-Content $appConfigPath -Raw
$oldMatch = [regex]::Match($content, 'VersionText\s*=\s*"([^"]+)"')
$oldVersion = $oldMatch.Groups[1].Value

$content = $content -replace '(VersionText\s*=\s*")[^"]+"', "`${1}$Version`""
Set-Content $appConfigPath $content -NoNewline

Write-Host ""
Write-Host "  CreAppConfig.cs" -ForegroundColor White
Write-Host "    VersionText: $oldVersion -> $Version" -ForegroundColor Green

# ── AssemblyInfo.cs ──────────────────────────────────────────────────
$content = Get-Content $assemblyInfoPath -Raw
$oldAssemblyMatch = [regex]::Match($content, 'AssemblyVersion\("([^"]+)"\)')
$oldAssemblyVersion = $oldAssemblyMatch.Groups[1].Value

$content = $content -replace '(AssemblyVersion\(")[^"]+"\)', "`${1}$fourPart`")"
$content = $content -replace '(AssemblyFileVersion\(")[^"]+"\)', "`${1}$fourPart`")"
Set-Content $assemblyInfoPath $content -NoNewline

Write-Host ""
Write-Host "  AssemblyInfo.cs" -ForegroundColor White
Write-Host "    AssemblyVersion: $oldAssemblyVersion -> $fourPart" -ForegroundColor Green
Write-Host "    AssemblyFileVersion: $oldAssemblyVersion -> $fourPart" -ForegroundColor Green

# ── .csproj ──────────────────────────────────────────────────────────────────
$content = Get-Content $csprojPath -Raw
$oldCsprojMatch = [regex]::Match($content, '<ApplicationVersion>([^<]+)</ApplicationVersion>')
$oldCsprojVersion = $oldCsprojMatch.Groups[1].Value
$oldRevisionMatch = [regex]::Match($content, '<ApplicationRevision>([^<]+)</ApplicationRevision>')
$oldRevision = $oldRevisionMatch.Groups[1].Value

$content = $content -replace '(<ApplicationVersion>)[^<]+(</ApplicationVersion>)', "`${1}$majorMinor`${2}"
$content = $content -replace '(<ApplicationRevision>)[^<]+(</ApplicationRevision>)', "`${1}$appRevision`${2}"
Set-Content $csprojPath $content -NoNewline

Write-Host ""
Write-Host "  ChaosRecipeEnhancer.UI.csproj" -ForegroundColor White
Write-Host "    ApplicationVersion: $oldCsprojVersion -> $majorMinor" -ForegroundColor Green
Write-Host "    ApplicationRevision: $oldRevision -> $appRevision" -ForegroundColor Green

# ── Summary ──────────────────────────────────────────────────────────
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Done! Files updated:" -ForegroundColor Cyan
Write-Host "  - src/App/.../CreAppConfig.cs         ($Version)" -ForegroundColor White
Write-Host "  - src/App/.../AssemblyInfo.cs          ($fourPart)" -ForegroundColor White
Write-Host "  - src/App/.../ChaosRecipeEnhancer.UI.csproj  (v$majorMinor rev$appRevision)" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Commit: git commit -am 'chore: bump version to $Version'" -ForegroundColor White
Write-Host "  2. PR release branch -> develop -> main" -ForegroundColor White
Write-Host "  3. GitHub Actions will handle the rest" -ForegroundColor White
Write-Host ""
