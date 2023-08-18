Write-Host ""
Write-Host "** Building API (.NET) Project **"
Write-Host ""
Write-Host "Environment variables used:"
Write-Host "===================================="
Write-Host "CRE_LOCAL_PATH: $env:CRE_LOCAL_PATH"
Write-Host "CRE_BUILD_PATH: $env:CRE_BUILD_PATH"

## Build
$projFiles = Get-ChildItem -Path $env:CRE_LOCAL_PATH/src/API/ChaosRecipeEnhancer.API -Filter *.csproj -Recurse

foreach ($projFile in $projFiles) {
    $dirName = [System.IO.Path]::GetDirectoryName($projFile.FullName) | Split-Path -Leaf

    Write-Host ""
    Write-Host "Building Project: $dirName"
    Write-Host ""

    Set-Location $projFile.DirectoryName
    
    dotnet build -c Release -o "$env:CRE_BUILD_PATH\$dirName\release\publish"

    Write-Host "The $dirName Application successfully built to $env:CRE_BUILD_PATH\$dirName\release\publish"
}

Write-Host ""
Write-Host "API Build Successful!"
Write-Host ""
