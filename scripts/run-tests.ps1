param([switch]$Headed)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$zone = [TimeZoneInfo]::FindSystemTimeZoneById('SE Asia Standard Time')
$localTime = [TimeZoneInfo]::ConvertTime([DateTimeOffset]::UtcNow, $zone)
$runId = $localTime.ToString('yyyyMMdd_HHmmss_fff')
$runDir = Join-Path $root "runs/$runId"
New-Item -ItemType Directory -Path $runDir -Force | Out-Null
$allureResults = Join-Path $runDir 'allure-results'
$env:ALLURE_CONFIG = Join-Path $runDir 'allureConfig.json'
$env:UTC_EVIDENCE_DIR = Join-Path $runDir 'screenshots'
$env:UTC_HEADLESS = if ($Headed) { 'false' } else { 'true' }
@{allure=@{directory=$allureResults}} | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $env:ALLURE_CONFIG -Encoding utf8
dotnet test (Join-Path $root 'UTC.LoginTests/UTC.LoginTests.csproj') --logger 'trx;LogFileName=results.trx' --results-directory $runDir
$testExit = $LASTEXITCODE
py -X utf8 (Join-Path $root 'scripts/update_execution.py') $runDir $runId
Write-Output "Run ID: $runId"
Write-Output "Results: $runDir"
exit $testExit
