$date = Get-Date
$version = $date.ToString("yyyy-dd-M--HH-mm-ss")
$filename = "StepStarterLambda-" + $version + ".zip"
cd .\StepStarterLambda\src\StepStarterLambda
dotnet lambda package ..\..\..\Packages\$filename --configuration Release -frun dotnet6 -farch arm64
cd ..\..\..