#!/bin/bash
set -ev

# build dotnet application
dotnet publish
# dotnet publish ./src/Messaging.Host/ -c release
# dotnet publish ./src/Scheduling.Host/ -c release

# determine which branch this change is from
# Support both Travis CI and GitHub Actions
if [ -n "$TRAVIS_PULL_REQUEST" ]; then
  # Travis CI
  if [ "${TRAVIS_PULL_REQUEST}" = "false" ]; then
    GIT_BRANCH=$TRAVIS_BRANCH
  else
    GIT_BRANCH=$TRAVIS_PULL_REQUEST_BRANCH
  fi
  BUILD_NUMBER=$TRAVIS_BUILD_NUMBER
elif [ -n "$GITHUB_ACTIONS" ]; then
  # GitHub Actions
  if [ "$GITHUB_EVENT_NAME" = "pull_request" ]; then
    GIT_BRANCH=$GITHUB_HEAD_REF
  else
    GIT_BRANCH=$GITHUB_REF_NAME
  fi
  BUILD_NUMBER=$GITHUB_RUN_NUMBER
else
  echo "Unknown CI environment"
  exit 1
fi

# create docker image(s)
docker login -u $DOCKER_HUB_USERNAME -p $DOCKER_HUB_PASSWORD
docker build --no-cache -t linn/print-service:$BUILD_NUMBER --build-arg gitBranch=$GIT_BRANCH ./src/Service.Host/

# push to dockerhub 
docker push linn/print-service:$BUILD_NUMBER
