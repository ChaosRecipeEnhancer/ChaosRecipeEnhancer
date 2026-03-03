# Extract Maven-specific sound data from FilterBlade's minified JS
$jsPath = "C:\Users\mario\.local\share\opencode\tool-output\tool_cb1daaf7c001Y5Nd2cosaN160A"
$content = [System.IO.File]::ReadAllText($jsPath)

# Find all .ogg filenames referenced
$oggMatches = [regex]::Matches($content, '"([a-z0-9]+\.ogg)"')
if ($oggMatches.Count -gt 0) {
    Write-Host "=== All .ogg files referenced in JS ($($oggMatches.Count) matches) ==="
    $uniqueFiles = @{}
    foreach ($m in $oggMatches) {
        $file = $m.Groups[1].Value
        if (-not $uniqueFiles.ContainsKey($file)) {
            $uniqueFiles[$file] = $true
            Write-Host $file
        }
    }
    Write-Host ""
    Write-Host "Total unique .ogg files: $($uniqueFiles.Count)"
}

# Also find the Maven sound array specifically - look for array containing .ogg entries
$mavenArrayMatch = [regex]::Match($content, '\[(?:"[a-z0-9]+\.ogg"(?:,"[a-z0-9]+\.ogg")*)\]')
if ($mavenArrayMatch.Success) {
    Write-Host ""
    Write-Host "=== Maven sound array (first match) ==="
    Write-Host $mavenArrayMatch.Value
}

# Look for Gollum/Kermit folder name mapping
$gollumMatches = [regex]::Matches($content, '.{0,80}Gollum.{0,80}')
Write-Host ""
Write-Host "=== Gollum context (first 3 matches) ==="
$count = 0
foreach ($m in $gollumMatches) {
    if ($count -ge 3) { break }
    Write-Host $m.Value
    Write-Host "---"
    $count++
}
