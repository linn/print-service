#!/bin/bash
set -ev

# build dotnet application
dotnet publish ./src/Service.Host/ -c Release
dotnet publish ./src/Messaging.Host/ -c Release

# determine which branch this change is from
if [ "${TRAVIS_PULL_REQUEST}" = "false" ]; then
  GIT_BRANCH=$TRAVIS_BRANCH
else
  GIT_BRANCH=$TRAVIS_PULL_REQUEST_BRANCH
fi

# create docker image(s)
docker login -u $DOCKER_HUB_USERNAME -p $DOCKER_HUB_PASSWORD
docker build --no-cache -t linn/print-service:$TRAVIS_BUILD_NUMBER --build-arg gitBranch=$GIT_BRANCH ./src/Service.Host/
docker build --no-cache -t linn/print-service-messaging:$TRAVIS_BUILD_NUMBER --build-arg gitBranch=$GIT_BRANCH ./src/Messaging.Host/

# push to dockerhub 
docker push linn/print-service:$TRAVIS_BUILD_NUMBER
docker push linn/print-service-messaging:$TRAVIS_BUILD_NUMBER
