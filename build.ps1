$version="1.0.0"

cd RaBe
Remove-Item -Path bin/Release -Recurse
mkdir bin/Release
dotnet restore RaBe.csproj

dotnet publish RaBe.csproj --framework netcoreapp3.0 -c Release -o bin/Release/RaBe /p:VersionSuffix=$version
dotnet publish RaBe.csproj --framework netcoreapp3.0 -c Release -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/RaBe-win64 /p:VersionSuffix=$version
dotnet publish RaBe.csproj --framework netcoreapp3.0 -c Release -r win-x86 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/RaBe-win32 /p:VersionSuffix=$version
dotnet publish RaBe.csproj --framework netcoreapp3.0 -c Release -r linux-arm /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/RaBe-linux-arm32v7 /p:VersionSuffix=$version
dotnet publish RaBe.csproj --framework netcoreapp3.0 -c Release -r win-arm /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/RaBe-win-arm32v7 /p:VersionSuffix=$version
dotnet publish RaBe.csproj --framework netcoreapp3.0 -c Release -r win-arm64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/RaBe-win-arm64 /p:VersionSuffix=$version
dotnet publish RaBe.csproj --framework netcoreapp3.0 -c Release -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/RaBe-linux64 /p:VersionSuffix=$version
dotnet publish RaBe.csproj --framework netcoreapp3.0 -c Release -r debian-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/RaBe-debian64 /p:VersionSuffix=$version
dotnet publish RaBe.csproj --framework netcoreapp3.0 -c Release -r ubuntu-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/RaBe-ubuntu64 /p:VersionSuffix=$version
dotnet publish RaBe.csproj --framework netcoreapp3.0 -c Release -r osx-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/RaBe-osx64 /p:VersionSuffix=$version

Copy-Item ../README.md bin/Release/RaBe/README.md
Copy-Item ../README.md bin/Release/RaBe-win64/README.md
Copy-Item ../README.md bin/Release/RaBe-win32/README.md
Copy-Item ../README.md bin/Release/RaBe-linux-arm32v7/README.md
Copy-Item ../README.md bin/Release/RaBe-win-arm32v7/README.md
Copy-Item ../README.md bin/Release/RaBe-win-arm64/README.md
Copy-Item ../README.md bin/Release/RaBe-linux64/README.md
Copy-Item ../README.md bin/Release/RaBe-debian64/README.md
Copy-Item ../README.md bin/Release/RaBe-ubuntu64/README.md
Copy-Item ../README.md bin/Release/RaBe-osx64/README.md

Copy-Item ../LICENSE bin/Release/RaBe/LICENSE
Copy-Item ../LICENSE bin/Release/RaBe-win64/LICENSE
Copy-Item ../LICENSE bin/Release/RaBe-win32/LICENSE
Copy-Item ../LICENSE bin/Release/RaBe-linux-arm32v7/LICENSE
Copy-Item ../LICENSE bin/Release/RaBe-win-arm32v7/LICENSE
Copy-Item ../LICENSE bin/Release/RaBe-win-arm64/LICENSE
Copy-Item ../LICENSE bin/Release/RaBe-linux64/LICENSE
Copy-Item ../LICENSE bin/Release/RaBe-debian64/LICENSE
Copy-Item ../LICENSE bin/Release/RaBe-ubuntu64/LICENSE
Copy-Item ../LICENSE bin/Release/RaBe-osx64/LICENSE

mkdir bin/Release/ZIPs

Compress-Archive -Path bin/Release/RaBe -DestinationPath bin/Release/ZIPs/RaBe.zip
Compress-Archive -Path bin/Release/RaBe-win64 -DestinationPath bin/Release/ZIPs/RaBe-win64.zip
Compress-Archive -Path bin/Release/RaBe-win32 -DestinationPath bin/Release/ZIPs/RaBe-win32.zip
Compress-Archive -Path bin/Release/RaBe-linux-arm32v7 -DestinationPath bin/Release/ZIPs/RaBe-linux-arm32v7.zip
Compress-Archive -Path bin/Release/RaBe-win-arm32v7 -DestinationPath bin/Release/ZIPs/RaBe-win-arm32v7.zip
Compress-Archive -Path bin/Release/RaBe-win-arm64 -DestinationPath bin/Release/ZIPs/RaBe-win-arm64.zip
Compress-Archive -Path bin/Release/RaBe-linux64 -DestinationPath bin/Release/ZIPs/RaBe-linux64.zip
Compress-Archive -Path bin/Release/RaBe-debian64 -DestinationPath bin/Release/ZIPs/RaBe-debian64.zip
Compress-Archive -Path bin/Release/RaBe-ubuntu64 -DestinationPath bin/Release/ZIPs/RaBe-ubuntu64.zip
Compress-Archive -Path bin/Release/RaBe-osx64 -DestinationPath bin/Release/ZIPs/RaBe-osx64.zip