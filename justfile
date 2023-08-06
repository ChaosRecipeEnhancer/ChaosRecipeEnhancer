# use PowerShell instead of sh:
set shell := ["powershell.exe", "-c"]

# if you're running into permission issues when running the scripts, follow these instructions:
# https://stackoverflow.com/a/70056419/10072406

setup-env-variables:
  .\scripts\setup-env-variables.ps1

build-api: setup-env-variables
  .\scripts\api\build.ps1

synth-infra: build-api
  .\scripts\infra\synth.ps1

deploy-infra: build-api
  .\scripts\infra\deploy.ps1