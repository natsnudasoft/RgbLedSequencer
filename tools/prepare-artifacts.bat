@echo off
echo ------------ Preparing artifacts ------------
robocopy "%~dp0..\docs\RgbLedSequencerLibrary\Help" "%~dp0..\src\RgbLedSequencerLibrary\bin\Release" Natsnudasoft.RgbLedSequencerLibrary.xml /NDL /NJH /NJS /NP /NS /NC
robocopy "%~dp0..\src\RgbLedSequencerLibrary\bin\Release" artifact\RgbLedSequencerLibrary *.dll *.xml *.pdb /XF *.dll.CodeAnalysisLog.xml /NDL /NJH /NJS /NP /NS /NC
robocopy "%~dp0..\src\RgbLedSequencer" artifact\RgbLedSequencer RgbLedSequencer.bas RgbLedSequencerCircuitDiagram.png /NDL /NJH /NJS /NP /NS /NC
7z a RgbLedSequencerLibrary_Release_Any_CPU.zip .\artifact\*
IF %errorlevel% LEQ 1 echo ------------ Artifacts prepared ------------
IF %errorlevel% LEQ 1 exit /B 0