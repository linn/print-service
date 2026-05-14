#!/bin/bash
set -ev

# build dotnet application
dotnet publish ./src/Service.Host/ -c Release
dotnet publish ./src/Messaging.Host/ -c Release

# determine which branch this change is from
if [ "${GITHUB_EVENT_NAME}" = "pull_request" ]; then
  GIT_BRANCH=$GITHUB_HEAD_REF
else
  GIT_BRANCH=${GITHUB_REF#refs/heads/}
fi

BUILD_NUMBER=$((LAST_TRAVIS_BUILD_NUMBER + GITHUB_RUN_NUMBER))

# create docker image(s)
docker login -u $DOCKER_HUB_USERNAME -p $DOCKER_HUB_PASSWORD
docker build --no-cache -t linn/print-service:$BUILD_NUMBER --build-arg gitBranch=$GIT_BRANCH ./src/Service.Host/
docker build --no-cache -t linn/print-service-messaging:$BUILD_NUMBER --build-arg gitBranch=$GIT_BRANCH ./src/Messaging.Host/

# push to dockerhub 
docker push linn/print-service:$BUILD_NUMBER
docker push linn/print-service-messaging:$BUILD_NUMBER
