Write-Host "Starting"
$publishPath = ".\publish"
if(Test-Path $publishPath) {
    Remove-Item -Path $publishPath -Recurse
}
New-Item -ItemType directory -Path $publishPath

& 'DummyGrpc\publish.ps1'
& 'DummyRest\publish.ps1'
& 'Meter\publish.ps1'

Move-Item -Path .\DummyGrpc\bin\publish.zip -Destination .\publish\grpc_server.zip
Move-Item -Path .\DummyRest\bin\publish.zip -Destination .\publish\rest_server.zip
Move-Item -Path .\Meter\bin\publish.zip -Destination .\publish\meter.zip