param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$CheckIn,

    [string]$Note = ""
)

$date = Get-Date -Format "yyyy-MM-dd"
$logsDir = Join-Path -Path (Get-Location) -ChildPath "logs"
$filePath = Join-Path -Path $logsDir -ChildPath "$date.md"

if (-not (Test-Path -LiteralPath $logsDir)) {
    New-Item -ItemType Directory -Path $logsDir | Out-Null
}

if (Test-Path -LiteralPath $filePath) {
    Write-Error "A check-in already exists for ${date}: $filePath"
    exit 1
}

$lines = @(
    "# $date",
    "",
    "- Check-in: $CheckIn"
)

if (-not [string]::IsNullOrWhiteSpace($Note)) {
    $lines += "- Note: $Note"
}

$lines | Set-Content -LiteralPath $filePath -Encoding UTF8

Write-Host "Created $filePath"
