# download-filterblade-sounds.ps1
# Downloads all community sound packs from FilterBlade.xyz for bundling into ChaosRecipeEnhancer.
# Source: https://www.filterblade.xyz/assets/communitySounds/{Author}/{filename}
# Data extracted from filterblade.min.js (communitySoundMembers, communitySoundFileNames)

param(
    [string]$OutputDir = "$PSScriptRoot\..\src\App\ChaosRecipeEnhancer.UI\Assets\Sounds\FilterSounds\Community"
)

$ErrorActionPreference = "Stop"
$baseUrl = "https://www.filterblade.xyz/assets/communitySounds"

# Standard authors - each has the same 9 .mp3 files
$standardAuthors = @(
    "Asuzara",
    "BexBloopers",
    "Brittleknee",
    "Chistor",
    "GhazzyTV",
    "Golaya",
    "Holly",
    "Lolcohol",
    "Mathil",
    "Mathil-vulgarity",
    "SlipperyJim",
    "StefanGold",
    "Gilbertamie",
    "Veskara",
    "Zizaran"
)

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

# ItsYoji has standard files + Slurp.mp3
$itsYojiFiles = $standardFiles + @("Slurp.mp3")

# Maven has a completely different set of .ogg files
$mavenFiles = @(
    "ah1.ogg", "ah2.ogg", "allthespecialones.ogg", "amazing.ogg", "atlasdelights.ogg",
    "beautiful.ogg", "beautiful1.ogg", "beautiful2.ogg", "captivating.ogg",
    "chaos1.ogg", "chaos2.ogg", "chaos3.ogg", "colleciton.ogg", "collectanother.ogg",
    "collectiongrows.ogg", "delicious.ogg", "divine1.ogg", "divine2.ogg", "divine3.ogg",
    "entertaining.ogg", "entertainingdirty.ogg", "entertainmemoan.ogg", "ex.ogg",
    "excitement.ogg", "exoticgifts.ogg", "exquisite.ogg", "fascinating.ogg",
    "favoriteoutcome.ogg", "favoriteoutcome1.ogg", "ferocious.ogg", "flashofpleasure.ogg",
    "flashy.ogg", "flashy1.ogg", "giftofstrength.ogg", "glorious1.ogg", "glorious2.ogg",
    "gloriousspectacle1.ogg", "gloriousspectacle2.ogg", "good1.ogg", "good2.ogg", "good3.ogg",
    "happy1.ogg", "hiding1.ogg", "hilarious.ogg", "ineedmore.ogg", "invitation.ogg",
    "iwantmore.ogg", "joy.ogg", "joygift.ogg", "magnificent1.ogg", "magnificent2.ogg",
    "massmurder.ogg", "mine1.ogg", "minemine1.ogg", "minenow.ogg",
    "moan1.ogg", "moan2.ogg", "moan3.ogg", "moan4.ogg", "moan5.ogg", "moan6.ogg",
    "moan7.ogg", "moan8.ogg", "moan9.ogg", "morepower.ogg", "mysteriouspresence.ogg",
    "nevertired.ogg", "newtoy.ogg", "notgood.ogg", "oh1.ogg", "oh2.ogg",
    "onemore1.ogg", "onemore2.ogg", "partofcollection.ogg", "plaything.ogg",
    "powerisfun.ogg", "raresight.ogg", "raretreat.ogg", "rush1.ogg", "rush2.ogg",
    "satisfactory.ogg", "savoureddelight.ogg", "song.ogg", "special1.ogg", "special2.ogg",
    "spectacular1.ogg", "spectacular2.ogg", "splashandsplatter.ogg", "suchpleasure.ogg",
    "surprising.ogg", "sweetsweet1.ogg", "sweetsweet2.ogg", "therewillbedeath.ogg",
    "uncommonmight.ogg", "unexpectednotunwelcome.ogg", "vibrant.ogg",
    "victoriousplaything.ogg", "victorygift.ogg", "wintribute.ogg",
    "withoutcompare1.ogg", "withoutcompare2.ogg", "worthit1.ogg", "worthit2.ogg",
    "yes1.ogg", "yes2.ogg", "yes3.ogg", "yesmine.ogg"
)

function Download-SoundPack {
    param(
        [string]$Author,
        [string[]]$Files
    )

    $authorDir = Join-Path $OutputDir $Author
    if (-not (Test-Path $authorDir)) {
        New-Item -ItemType Directory -Force -Path $authorDir | Out-Null
    }

    $downloaded = 0
    $skipped = 0
    $failed = 0

    foreach ($file in $Files) {
        $outPath = Join-Path $authorDir $file
        if (Test-Path $outPath) {
            $skipped++
            continue
        }

        $url = "$baseUrl/$Author/$file"
        try {
            Invoke-WebRequest -Uri $url -OutFile $outPath -UseBasicParsing
            $downloaded++
        }
        catch {
            Write-Warning "  Failed to download $url : $_"
            $failed++
        }
    }

    Write-Host "  $Author : $downloaded downloaded, $skipped skipped (already exist), $failed failed"
}

# Ensure output directory exists
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
}

Write-Host "Downloading FilterBlade community sounds to: $OutputDir"
Write-Host ""

# Download standard packs
Write-Host "=== Standard Packs (15 authors x 9 files) ==="
foreach ($author in $standardAuthors) {
    Download-SoundPack -Author $author -Files $standardFiles
}

# Download ItsYoji (standard + extra)
Write-Host ""
Write-Host "=== ItsYoji (10 files) ==="
Download-SoundPack -Author "ItsYoji" -Files $itsYojiFiles

# Download Maven (103 .ogg files)
Write-Host ""
Write-Host "=== Maven (103 .ogg files) ==="
Download-SoundPack -Author "Maven" -Files $mavenFiles

Write-Host ""
Write-Host "Done! All community sounds downloaded to: $OutputDir"
