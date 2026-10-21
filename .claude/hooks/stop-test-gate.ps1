# Stop: refuse to finish while tests are red. Skipped on a retry to avoid an endless loop.
$payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
if ($payload.stop_hook_active) { exit 0 }

# Nothing to test until a test project exists with at least one test.
if (-not (Test-Path 'tests')) { exit 0 }

$output = dotnet test --nologo -v q 2>&1 | Out-String
if ($LASTEXITCODE -ne 0) {
    [Console]::Error.WriteLine("dotnet test failed; fix before finishing.`n$output")
    exit 2
}
