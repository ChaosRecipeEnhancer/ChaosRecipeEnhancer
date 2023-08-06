Write-Host ""
Write-Host "** Checking / Setting Environment Variables **"
Write-Host ""

$current_local_path = $PWD

# Check and set CRE_LOCAL_PATH environment variable
if ($env:CRE_LOCAL_PATH -and $env:CRE_LOCAL_PATH -eq $current_local_path) {
    Write-Host "CRE_LOCAL_PATH environment variable already exists."
}
else {
    [System.Environment]::SetEnvironmentVariable('CRE_LOCAL_PATH', $current_local_path, 'User')
    Write-Host "CRE_LOCAL_PATH environment variable set."
}

# Check and set CRE_BUILD_PATH environment variable
$current_build_path = Join-Path $current_local_path 'dist'
if ($env:CRE_BUILD_PATH -and $env:CRE_BUILD_PATH -eq $current_build_path) {
    Write-Host "CRE_BUILD_PATH environment variable already exists."
}
else {
    [System.Environment]::SetEnvironmentVariable('CRE_BUILD_PATH', $current_build_path, 'User')
    Write-Host "CRE_BUILD_PATH environment variable set."
}

Write-Host ""
Write-Host "Environment variables checked / set:"
Write-Host "CRE_LOCAL_PATH: $current_local_path"
Write-Host "CRE_BUILD_PATH: $current_build_path"
Write-Host ""
Write-Host "Only future PowerShell sessions will see this variable - you will need to restart your PowerShell instance if you'd like to access them."
Write-Host "If you're using an integrated terminal, you'll need to restart your IDE if you'd like to access them there."
