@echo on
pause
dotnet restore
pause
dotnet publish -r centos.7-x64 -c release
pause
