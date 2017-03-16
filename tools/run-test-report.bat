@echo off
SET openCoverVersion=4.6.519
SET xunitRunnerVersion=2.2.0
SET reportGeneratorVersion=2.5.6
..\packages\OpenCover.%openCoverVersion%\tools\OpenCover.Console.exe -register:user "-filter:+[*]* -[*Tests]*" -target:..\packages\xunit.runner.console.%xunitRunnerVersion%\tools\xunit.console.exe -targetargs:"..\test\unit\RgbLedSequencerLibraryTests\bin\Debug\RgbLedSequencerLibraryTests.dll -noshadow" -excludebyattribute:*.ExcludeFromCodeCoverage* -excludebyfile:*Designer.cs -output:coverage.xml && ^
..\packages\ReportGenerator.%reportGeneratorVersion%\tools\ReportGenerator.exe -reports:coverage.xml -targetdir:coverage && ^
echo Press any key to display report... && ^
pause >nul && ^
start coverage\index.htm