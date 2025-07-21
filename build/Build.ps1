Write-Host "Cleaning..."
Remove-Item ..\src\lib\EyeSpy\bin -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item ..\src\lib\EyeSpy\obj -Recurse -Force -ErrorAction SilentlyContinue
dotnet clean ..\src\lib\EyeSpy\EyeSpy.csproj

Write-Host "Restoring..."
dotnet restore ..\src\lib\EyeSpy\EyeSpy.csproj

Write-Host "Building..."
dotnet build ..\src\lib\EyeSpy\EyeSpy.csproj -c Debug -v:n

Write-Host "Packing..."
dotnet pack ..\src\lib\EyeSpy\EyeSpy.csproj -c Release -o ../dist

Write-Host "Clearing local NuGet cache..."
dotnet nuget locals all --clear