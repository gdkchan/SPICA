@echo off
cls
echo Compiling SPICA...
"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild" SPICA.sln /p:Configuration=Release
echo Build is complete, you also need OpenTK 2.0 to run it:
echo https://www.nuget.org/packages/OpenTK/
pause