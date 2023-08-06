Write-Host ""
Write-Host "** Deploying Infrastructure **"
Write-Host ""
Write-Host "Environment variables used:"
Write-Host "===================================="
Write-Host "LOCAL_PATH: $env:CRE_LOCAL_PATH"

Set-Location $env:CRE_LOCAL_PATH/src/Infra
npx cdk deploy --require-approval never
