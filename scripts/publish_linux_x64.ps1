# Publish and Zip script for CMDocumentRepository — Linux x64
$ErrorActionPreference = "Stop"

$basePath = "C:\dev\CMDocumentRepository"
$runtime = "linux-x64"
$releaseDir = "$basePath\Release"

$deployName = "CMDocumentRepository"

$projects = @(
    @{ Name = "CMDocumentRepository.Presentation"; Path = "$basePath\CMDocumentRepository.Presentation\CMDocumentRepository.Presentation.csproj"; Out = "$releaseDir\$deployName" }
)

# Ensure release directory exists
if (-not (Test-Path $releaseDir)) {
    New-Item -ItemType Directory -Path $releaseDir -Force | Out-Null
}

foreach ($project in $projects) {
    Write-Host "--- Building $($project.Name) for $runtime ---" -ForegroundColor Cyan

    # 1. Clean previous output
    if (Test-Path $project.Out) {
        Write-Host "Cleaning previous output: $($project.Out)" -ForegroundColor DarkGray
        Remove-Item $project.Out -Recurse -Force
    }

    # 2. Publish
    dotnet publish $project.Path -c Release -r $runtime --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o $project.Out

    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: dotnet publish failed with exit code $LASTEXITCODE" -ForegroundColor Red
        exit $LASTEXITCODE
    }

    # 3. Create ZIP
    $zipPath = Join-Path $releaseDir "$deployName.zip"
    if (Test-Path $zipPath) { Remove-Item $zipPath }

    Write-Host "Archiving to $([System.IO.Path]::GetFileName($zipPath))..." -ForegroundColor Yellow

    Push-Location $project.Out
    try {
        # Files and folders to include in the deployment archive
        $itemsToZip = @()

        # Single-file executable (no extension on Linux)
        if (Test-Path $deployName) { $itemsToZip += $deployName }

        # Configuration
        if (Test-Path "appsettings.json") { $itemsToZip += "appsettings.json" }

        # Static assets (CSS, JS, images)
        if (Test-Path "wwwroot") { $itemsToZip += "wwwroot" }

        # Razor views (required for MVC at runtime)
        if (Test-Path "Views") { $itemsToZip += "Views" }

        Add-Type -AssemblyName "System.IO.Compression.FileSystem"
        $zipArchive = [System.IO.Compression.ZipFile]::Open($zipPath, "Create")

        foreach ($item in $itemsToZip) {
            if (Test-Path $item -PathType Container) {
                $files = Get-ChildItem $item -Recurse
                foreach ($file in $files) {
                    if (-not $file.PSIsContainer) {
                        $relativeName = $file.FullName.Substring($project.Out.Length + 1).Replace('\', '/')
                        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zipArchive, $file.FullName, $relativeName)
                    }
                }
            }
            else {
                $relativeName = $item.Replace('\', '/')
                [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zipArchive, (Join-Path $project.Out $item), $relativeName)
            }
        }
        # Install script (from scripts/ directory, added at zip root)
        $installScript = Join-Path $basePath "scripts\install.sh"
        if (Test-Path $installScript) {
            [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zipArchive, $installScript, "install.sh")
        }

        $zipArchive.Dispose()
    }
    finally {
        Pop-Location
    }

    $zipSize = "{0:N1} MB" -f ((Get-Item $zipPath).Length / 1MB)
    Write-Host "Done: $zipPath ($zipSize)" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== Publish complete ===" -ForegroundColor Green
Write-Host "Output directory : $releaseDir\$deployName" -ForegroundColor Gray
Write-Host "Deployment archive: $releaseDir\$deployName.zip" -ForegroundColor Gray
