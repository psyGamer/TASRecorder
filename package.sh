rm -rd bin obj
# Compiling in Release mode causes some issues
dotnet publish -c Debug
zip -r TASRecorder-v$1.zip everest.yaml bin/TASRecorder.dll bin/TASRecorder.pdb Dialog Graphics
