# Convert all .ogg files in Maven community sounds to .mp3 using ffmpeg
# Requires ffmpeg to be installed and on PATH

param(
    [string]$MavenDir = "$PSScriptRoot\..\src\App\ChaosRecipeEnhancer.UI\Assets\Sounds\FilterSounds\Community\Maven"
)

$ErrorActionPreference = "Continue"

# Check ffmpeg
$ffmpeg = Get-Command ffmpeg -ErrorAction SilentlyContinue
if (-not $ffmpeg) {
    Write-Host "ffmpeg not found on PATH. Trying winget install..."
    winget install Gyan.FFmpeg --accept-package-agreements --accept-source-agreements
    # Refresh PATH
    $env:PATH = [System.Environment]::GetEnvironmentVariable("PATH", "Machine") + ";" + [System.Environment]::GetEnvironmentVariable("PATH", "User")
    $ffmpeg = Get-Command ffmpeg -ErrorAction SilentlyContinue
    if (-not $ffmpeg) {
        Write-Error "ffmpeg is required but could not be installed. Please install ffmpeg and try again."
        exit 1
    }
}

Write-Host "Converting .ogg files in: $MavenDir"

$oggFiles = Get-ChildItem -Path $MavenDir -Filter "*.ogg"
$converted = 0
$skipped = 0

foreach ($ogg in $oggFiles) {
    $mp3Path = [System.IO.Path]::ChangeExtension($ogg.FullName, ".mp3")

    if (Test-Path $mp3Path) {
        $skipped++
        continue
    }

    $output = & ffmpeg -i $ogg.FullName -ab 192k -y $mp3Path 2>&1
    if ($LASTEXITCODE -eq 0) {
        $converted++
    }
    else {
        Write-Host "  FAILED: $($ogg.Name)"
    }
}

# Remove original .ogg files after successful conversion
if ($converted -gt 0 -or $skipped -gt 0) {
    $oggToRemove = Get-ChildItem -Path $MavenDir -Filter "*.ogg" | Where-Object {
        Test-Path ([System.IO.Path]::ChangeExtension($_.FullName, ".mp3"))
    }
    foreach ($ogg in $oggToRemove) {
        Remove-Item $ogg.FullName
        Write-Host "  Removed: $($ogg.Name)"
    }
}

Write-Host ""
Write-Host "Done! Converted: $converted, Skipped (already .mp3): $skipped"
