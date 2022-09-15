$ErrorActionPreference = "Stop"

$resultsFile = "results.txt"

Push-Location .\AzureResourceDiscoveryCli\AzureResourceDiscovery
dotnet run -- -o $resultsFile -d .\ -f ..\..\manifest.json
if ($LastExitCode -ne 0) {
    Pop-Location
    throw "An error has occured. Unable to generate Azure policy file(s)."
}
$allFiles = Get-Content $resultsFile
foreach ($line in $allFiles) {
    
    $parts = $line.Split('|')
    
    $name = $parts[0]
    $displayName = $parts[1]
    $description = $parts[2]
    $filePath = $parts[3]

    Write-Output "Processing $filePath"

    az policy definition create --name $name --display-name  $displayName --description $description --rules $filePath --mode All

    Remove-Item $line -Force
    if (!$rootDir) {
        $rootDir = $line.Split('\')[1]
    }
}

Remove-Item $rootDir -Force
Remove-Item $resultsFile -Force
Pop-Location