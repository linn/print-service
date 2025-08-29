#!/bin/bash
set -ev

# upgrade node to latest version
if [ "$CI" ] && [ "$TRAVIS" ]
then 
	source ~/.nvm/nvm.sh; 
	nvm install 22;
	nvm use 22;
fi

echo $?
if [ $? -eq 1 ]; then
  echo dotnet test fail
  exit 1
fi

# javascript tests
cd ./src/Service.Host
npm ci
npm test
echo $?
result=$?
cd ../..

exit $result
