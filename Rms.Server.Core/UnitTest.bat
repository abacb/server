@echo off

REM dotnet.exe�̃t���p�X
SET DOTNET_PATH=C:\Program Files\dotnet\dotnet.exe
REM OpenCover.Console.exe�̃t���p�X
SET OPEN_COVER_PATH=%USERPROFILE%\.nuget\packages\opencover\4.7.922\tools\OpenCover.Console.exe
REM ReportGenerator.exe�̃t���p�X
SET REPORT_GEN=%USERPROFILE%\.nuget\packages\reportgenerator\4.5.6\tools\net47\ReportGenerator.exe
REM �P�̃e�X�g���ʂ̏o�̓t�H���_
SET OUTPUT_DIR=%~dp0\UnitTestResults

rd /s /q %OUTPUT_DIR%
mkdir %OUTPUT_DIR%\coverage
 
for /r %~dp0 %%f in (*Test.csproj) do (
  %OPEN_COVER_PATH% -register:user -target:"%DOTNET_PATH%" -targetargs:"test --no-build --logger trx;LogFileName=%OUTPUT_DIR%\%%~nf.trx %%f" -targetdir:"%%~dpfbin\Debug\netcoreapp2.1" -filter:"+[Rms.Server*]*" -output:%OUTPUT_DIR%\coverage\results.xml -mergeoutput -oldstyle
)

%REPORT_GEN% -reports:"%OUTPUT_DIR%\coverage\results.xml" -targetdir:%OUTPUT_DIR%\coverage
start "" %OUTPUT_DIR%\coverage\index.htm
