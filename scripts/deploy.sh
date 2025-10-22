#!/bin/bash
set -ev

echo "Installing AWS CLI..."
curl -s "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip -q awscliv2.zip
sudo ./aws/install >/dev/null 2>&1
echo "AWS CLI installed."

# Determine CI environment and set variables accordingly
if [ -n "$TRAVIS_BRANCH" ]; then
  # Travis CI
  BRANCH=$TRAVIS_BRANCH
  IS_PR=$TRAVIS_PULL_REQUEST
  BUILD_NUMBER=$TRAVIS_BUILD_NUMBER
elif [ -n "$GITHUB_ACTIONS" ]; then
  # GitHub Actions
  if [ "$GITHUB_EVENT_NAME" = "pull_request" ]; then
    BRANCH=$GITHUB_BASE_REF
    IS_PR="true"
  else
    BRANCH=$GITHUB_REF_NAME
    IS_PR="false"
  fi
  BUILD_NUMBER=$GITHUB_RUN_NUMBER
else
  echo "Unknown CI environment"
  exit 1
fi

# deploy on aws
if [ "${BRANCH}" = "main" ]; then
  if [ "${IS_PR}" = "false" ]; then
    # master - deploy to production
    echo deploy to production

    aws s3 cp s3://$S3_BUCKET_NAME/printService/production.env ./secrets.env

    STACK_NAME=printService
    APP_ROOT=http://app.linn.co.uk
    PROXY_ROOT=http://app.linn.co.uk
  	ENV_SUFFIX=
  else
    # pull request based on master - deploy to sys
    echo deploy to sys

    aws s3 cp s3://$S3_BUCKET_NAME/printService/sys.env ./secrets.env

    STACK_NAME=printService-sys
    APP_ROOT=http://app-sys.linn.co.uk
    PROXY_ROOT=http://app.linn.co.uk
    ENV_SUFFIX=-sys
  fi
fi


# load the secret variables but hide the output from the travis log
source ./secrets.env > /dev/null 2>&1

# deploy the service to amazon
aws cloudformation deploy --stack-name $STACK_NAME --template-file ./aws/application.yml --parameter-overrides dockerTag=$BUILD_NUMBER printUsername=$PRINT_USERNAME printPassword=$PRINT_PASSWORD loggingEnvironment=$LOG_ENVIRONMENT loggingMaxInnerExceptionDepth=$LOG_MAX_INNER_EXCEPTION_DEPTH environmentSuffix=$ENV_SUFFIX --capabilities=CAPABILITY_IAM

echo "deploy complete"
