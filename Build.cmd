@echo off
cls
dotnet clean src\Soukoku.Owin.Files -c Release
dotnet pack src\Soukoku.Owin.Files -c Release -o build
dotnet clean src\Soukoku.Owin.Files.Zipped -c Release
dotnet pack src\Soukoku.Owin.Files.Zipped -c Release -o build
pause