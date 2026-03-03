# download-builtin-sounds.ps1
# Downloads PoE's 16 built-in alert sounds from FilterBlade.xyz for preview in CRE.
# Source: https://www.filterblade.xyz/assets/sounds/AlertSound{N}.mp3

param(
    [string]$OutputDir = "$PSScriptRoot\..\src\App\ChaosRecipeEnhancer.UI\Assets\Sounds\FilterSounds\BuiltIn"
)

$ErrorActionPreference = "Stop"
$baseUrl = "https://www.filterblade.xyz/assets/sounds"

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
}

Write-Host "Downloading built-in PoE alert sounds to: $OutputDir"

for ($i = 1; $i -le 16; $i++) {
    $fileName = "AlertSound$i.mp3"
    $url = "$baseUrl/$fileName"
    $outPath = Join-Path $OutputDir $fileName

    if (Test-Path $outPath) {
        Write-Host "  $fileName : skipped (already exists)"
        continue
    }

    try {
        Invoke-WebRequest -Uri $url -OutFile $outPath -UseBasicParsing
        Write-Host "  $fileName : downloaded"
    }
    catch {
        Write-Host "  $fileName : FAILED - $_"
    }
}

Write-Host ""
Write-Host "Done!"
