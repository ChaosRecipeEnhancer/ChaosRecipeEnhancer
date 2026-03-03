# Download the two missing sound packs: Gollum (Zizaran) and Kermit (Zizaran)
$baseUrl = "https://www.filterblade.xyz/assets/communitySounds"
$outputBase = "$PSScriptRoot\..\src\App\ChaosRecipeEnhancer.UI\Assets\Sounds\FilterSounds\Community"

$standardFiles = @(
    "1maybevaluable.mp3",
    "2currency.mp3",
    "3uniques.mp3",
    "4maps.mp3",
    "5highmaps.mp3",
    "6veryvaluable.mp3",
    "7chancing.mp3",
    "12leveling.mp3",
    "placeholder.mp3"
)

# Server folder names include parentheses
$missingAuthors = @(
    @{ ServerName = "Gollum (Zizaran)"; LocalName = "Gollum (Zizaran)" },
    @{ ServerName = "Kermit (Zizaran)"; LocalName = "Kermit (Zizaran)" }
)

foreach ($author in $missingAuthors) {
    $serverName = $author.ServerName
    $localName = $author.LocalName
    $localDir = Join-Path $outputBase $localName

    if (-not (Test-Path $localDir)) {
        New-Item -ItemType Directory -Force -Path $localDir | Out-Null
    }

    Write-Host "Downloading: $serverName"
    foreach ($file in $standardFiles) {
        $outPath = Join-Path $localDir $file
        if (Test-Path $outPath) {
            Write-Host "  $file : skipped"
            continue
        }

        $url = "$baseUrl/$([uri]::EscapeDataString($serverName))/$file"
        try {
            Invoke-WebRequest -Uri $url -OutFile $outPath -UseBasicParsing
            Write-Host "  $file : downloaded"
        }
        catch {
            Write-Host "  $file : FAILED"
        }
    }
}

Write-Host ""
Write-Host "Done!"
