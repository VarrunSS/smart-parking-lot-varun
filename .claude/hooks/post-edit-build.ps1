# PostToolUse (Edit|Write): rebuild when a C# file changed so compile errors surface immediately.
$payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
$path = $payload.tool_input.file_path
if (-not $path -or $path -notlike '*.cs') { exit 0 }

$output = dotnet build --nologo -v q -warnaserror 2>&1 | Out-String
if ($LASTEXITCODE -ne 0) {
    [Console]::Error.WriteLine($output)
    exit 2
}
