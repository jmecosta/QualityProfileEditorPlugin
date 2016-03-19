@echo on
nuget restore
call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat"
call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VsDevCmd.bat"
msbuild BuildPlugin.msbuild /p:VisualStudioVersion=12.0 /p:AssemblyPatcherTaskOn=true /p:SkipCopy=No /p:EndVSQFile=QualityEditorPlugin2013.VSQ > buildlog2013.txt
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\vsvars32.bat"
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat"
msbuild BuildPlugin.msbuild /p:VisualStudioVersion=14.0 /p:AssemblyPatcherTaskOn=true /p:SkipCopy=No /p:EndVSQFile=QualityEditorPlugin2015.VSQ > buildlog2015.txt
