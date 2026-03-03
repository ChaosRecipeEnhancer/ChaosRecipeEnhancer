# Extract community sound data from FilterBlade's minified JS
$jsPath = "C:\Users\mario\.local\share\opencode\tool-output\tool_cb1daaf7c001Y5Nd2cosaN160A"
$content = [System.IO.File]::ReadAllText($jsPath)

# Extract communitySoundMembers array
if ($content -match 'communitySoundMembers\s*=\s*(\[.*?\])') {
    Write-Host "=== communitySoundMembers ==="
    Write-Host $matches[1]
    Write-Host ""
}

# Extract communitySoundFileNames array
if ($content -match 'communitySoundFileNames\s*=\s*(\[.*?\])') {
    Write-Host "=== communitySoundFileNames ==="
    Write-Host $matches[1]
    Write-Host ""
}

# Extract communitySoundFileUiNames array
if ($content -match 'communitySoundFileUiNames\s*=\s*(\[.*?\])') {
    Write-Host "=== communitySoundFileUiNames ==="
    Write-Host $matches[1]
    Write-Host ""
}

# Look for Maven sound data
if ($content -match '(Maven.*?\.ogg.*?\])') {
    Write-Host "=== Maven section ==="
    Write-Host $matches[1]
    Write-Host ""
}

# Look for any "communitySounds" path patterns
$csMatches = [regex]::Matches($content, 'communitySounds/\w+/[\w.-]+\.\w{3}')
if ($csMatches.Count -gt 0) {
    Write-Host "=== communitySounds URL patterns found ==="
    foreach ($m in $csMatches) {
        Write-Host $m.Value
    }
}
