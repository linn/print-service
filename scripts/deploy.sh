#!/bin/bash
set -ev

echo "Checking AWS CLI version..."
aws --version

# Handle GitHub Actions environment variables
if [ -n "${GITHUB_REF_NAME}" ]; then
  CURRENT_BRANCH="${GITHUB_REF_NAME}"
  if [ "${GITHUB_EVENT_NAME}" = "pull_request" ]; then
    IS_PULL_REQUEST="true"
  else
    IS_PULL_REQUEST="false"
  fi
fi

if [ "${CURRENT_BRANCH}" = "main" ] || [ "${GITHUB_BASE_REF}" = "main" ]; then
  if [ "${IS_PULL_REQUEST}" = "false" ]; then
    # main branch push - deploy to production
    echo deploy to production

    aws s3 cp s3://$S3_BUCKET_NAME/printService/production.env ./secrets.env

    STACK_NAME=printService
    APP_ROOT=http://app.linn.co.uk
    PROXY_ROOT=http://app.linn.co.uk
    ENV_SUFFIX=
  else
    # pull request to main - deploy to sys
    echo deploy to sys

    aws s3 cp s3://$S3_BUCKET_NAME/printService/sys.env ./secrets.env

    STACK_NAME=printService-sys
    APP_ROOT=http://app-sys.linn.co.uk
    PROXY_ROOT=http://app.linn.co.uk
    ENV_SUFFIX=-sys
  fi
else
  echo do not deploy
fi

# load the secret variables but hide the output
source ./secrets.env > /dev/null 2>&1

# use continuous build number (Travis + GitHub Actions)
LAST_TRAVIS_BUILD_NUMBER="${LAST_TRAVIS_BUILD_NUMBER:-0}"
BUILD_NUMBER=$((LAST_TRAVIS_BUILD_NUMBER + GITHUB_RUN_NUMBER))

# deploy the service to amazon
aws cloudformation deploy \
--stack-name $STACK_NAME \
--template-file ./aws/application.yml \
--parameter-overrides \
dockerTag=$BUILD_NUMBER \
databaseHost=$DATABASE_HOST \ 
databaseName=$DATABASE_NAME \
databaseUserId=$DATABASE_USER_ID \
databasePassword=$DATABASE_PASSWORD \
printUsername=$PRINT_USERNAME \
printPassword=$PRINT_PASSWORD \
environmentSuffix=$ENV_SUFFIX \
rabbitServer=$RABBIT_SERVER \
rabbitUsername=$RABBIT_USERNAME \
rabbitPassword=$RABBIT_PASSWORD \
rabbitPort=$RABBIT_PORT \
proxyRoot=$PROXY_ROOT \
--tags Name=$STACK_NAME CIT=IT \
--capabilities=CAPABILITY_IAM

echo "deploy complete"
