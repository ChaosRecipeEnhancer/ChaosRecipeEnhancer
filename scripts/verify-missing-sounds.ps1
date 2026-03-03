# Verify missing sound packs by probing FilterBlade URLs
$baseUrl = "https://www.filterblade.xyz/assets/communitySounds"
$testFile = "1maybevaluable.mp3"

# Test folder names for the missing packs
$testFolders = @("Gollum", "Kermit", "Gollum (Zizaran)", "Kermit (Zizaran)", "StefanGold", "Stefan Gold")

foreach ($folder in $testFolders) {
    $url = "$baseUrl/$folder/$testFile"
    try {
        $response = Invoke-WebRequest -Uri $url -Method Head -UseBasicParsing -ErrorAction Stop
        Write-Host "OK    : $folder/$testFile (status: $($response.StatusCode))"
    }
    catch {
        $status = $_.Exception.Response.StatusCode.value__
        Write-Host "FAIL  : $folder/$testFile (status: $status)"
    }
}

# Also verify our existing Zizaran folder actually has valid files
$url = "$baseUrl/Zizaran/$testFile"
try {
    $response = Invoke-WebRequest -Uri $url -Method Head -UseBasicParsing -ErrorAction Stop
    Write-Host "OK    : Zizaran/$testFile (status: $($response.StatusCode))"
}
catch {
    $status = $_.Exception.Response.StatusCode.value__
    Write-Host "FAIL  : Zizaran/$testFile (status: $status)"
}
