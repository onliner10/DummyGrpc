dotnet publish --self-contained -r linux-x64 -c Release

$binFolder = Join-Path $PSScriptRoot "bin"
Compress-Archive -Path $binFolder\Release\netcoreapp2.2\linux-x64\publish\* -DestinationPath $binFolder\publish.zip -Force