#!/bin/bash
set -ev

dotnet test ./tests/Unit/Unit.Tests/Unit.Tests.csproj
dotnet test ./tests/Integration/Integration.Tests/Integration.Tests.csproj

echo $?
if [ $? -eq 1 ]; then
  echo dotnet test fail
  exit 1
fi

exit $result
