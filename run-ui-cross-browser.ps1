
$projectPath = Join-Path $PSScriptRoot "OrangeHRM.Tests\OrangeHRM.Tests.csproj"
$resultsPath = Join-Path $PSScriptRoot "OrangeHRM.Tests\bin\Debug\net10.0\allure-results"
$allureRoot = Join-Path $PSScriptRoot "allure-results"

$browsers = @("Chrome", "Firefox")
$failedBrowsers = @()

foreach ($browser in $browsers) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Running UI tests in $browser" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan

    $env:Browser = $browser

    # Remove results from the previous run.
    if (Test-Path $resultsPath) {
        Remove-Item $resultsPath -Recurse -Force
    }

    # Run UI tests only.
    dotnet test $projectPath --filter "FullyQualifiedName!~OrangeHRM.Tests.API"
    $exitCode = $LASTEXITCODE

    # Preserve this browser's Allure results.
    $browserResultsPath = Join-Path $allureRoot $browser

    if (Test-Path $browserResultsPath) {
        Remove-Item $browserResultsPath -Recurse -Force
    }

    New-Item -Path $browserResultsPath -ItemType Directory -Force | Out-Null

    if (Test-Path $resultsPath) {
        $resultFiles = Get-ChildItem $resultsPath -Force

        if ($resultFiles) {
            Move-Item -Path $resultFiles.FullName -Destination $browserResultsPath
        }
    }

    if ($exitCode -ne 0) {
        $failedBrowsers += $browser
        Write-Host "$browser tests FAILED" -ForegroundColor Red
    }
    else {
        Write-Host "$browser tests PASSED" -ForegroundColor Green
    }
}

Remove-Item Env:\Browser -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "========== SUMMARY ==========" -ForegroundColor Cyan

if ($failedBrowsers.Count -gt 0) {
    Write-Host "Failed browsers: $($failedBrowsers -join ', ')" -ForegroundColor Red
    exit 1
}

Write-Host "All browser runs passed!" -ForegroundColor Green
Write-Host "Allure results: $allureRoot"
