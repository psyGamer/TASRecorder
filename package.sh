#!/usr/bin/env sh

rm -rd bin Source/bin Source/obj
dotnet build -c Release

if [ $# -eq 0 ]
  then
  zip -r TASRecorder.zip everest.yaml bin Dialog Graphics
else
  zip -r TASRecorder-v$1.zip everest.yaml bin Dialog Graphics
fi
