FROM mcr.microsoft.com/dotnet/aspnet:10.0

ARG gitBranch=unspecified
ENV TZ=Europe/London

RUN apt-get update && apt-get install -y curl
RUN rm -rf /var/lib/apt/lists/*

EXPOSE 5050

COPY bin/Release/net10.0/publish/ /app/bin/

ENV APP_PATH /app/
ENV GIT_BRANCH $gitBranch

CMD dotnet /app/bin/Linn.PrintService.Messaging.Host.dll
