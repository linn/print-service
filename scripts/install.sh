#!/bin/bash
set -ev

dotnet restore

# upgrade node to latest version
if [ "$CI" ] && [ "$TRAVIS" ]
then 
	source ~/.nvm/nvm.sh
	nvm install 22
	nvm use 22
fi

# GitHub Actions already sets up the correct Node version, no need to upgrade
if [ "$CI" ] && [ "$GITHUB_ACTIONS" ]
then
	echo "Node.js version already set up by GitHub Actions"
	node --version
fi
