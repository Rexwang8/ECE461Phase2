REM params ./winrun.bat <gh token>

del /F /Q bin\Release\netcoreapp2.0\publish 
dotnet publish src/Index/Index.csproj -c Release -o bin\Release\netcoreapp2.0\publish -p:PublishSingleFile=true --self-contained true
start "" bin/Release/netcoreapp2.0/publish/Index.exe E:\Projects\461pkg\ECE461Phase2\CLI\example\git.txt 2 E:\Projects\461pkg\ECE461Phase2\CLI\example\log.txt %1