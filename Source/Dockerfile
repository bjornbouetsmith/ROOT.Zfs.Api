# create build environment in /Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /Build

# Copy source files
COPY ./Api/* ./Api/

# Build and publish to /Build/out
RUN dotnet build Api/Api.csproj -c Release --framework net6.0 --runtime linux-x64

RUN dotnet publish Api/Api.csproj --no-build -c Release --framework net6.0 --sc --runtime linux-x64 -o out
CMD ls -l
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# install required packages
run apt-get update && apt-get -y install libpam0g-dev ssh

# Create app dir /App and copy publishing output to /App
WORKDIR /App
COPY --from=build-env /Build/out .

# create user to run our app
RUN groupadd -g 10001 dotnet && \
   useradd -u 10000 -g dotnet dotnet -rmd /home/dotnet \
   && chown -R dotnet:dotnet /App

ARG SSH_PRIVATE_KEY=id_rsa
ARG SSH_PUBLIC_KEY=id_rsa.pub

# copy ssh keys from build arguments
RUN mkdir -p /home/dotnet/.ssh
COPY ${SSH_PRIVATE_KEY} /home/dotnet/.ssh/id_rsa
COPY ${SSH_PUBLIC_KEY} /home/dotnet/.ssh/id_rsa.pub

# Make dotnet user owner of everything below /home/dotnet and set correct ssh permisssions on files
RUN chown -R dotnet:dotnet /home/dotnet && chmod 700 /home/dotnet/.ssh && chmod 644 /home/dotnet/.ssh/id_rsa.pub && chmod 600 /home/dotnet/.ssh/id_rsa

# Allow docker image to connect to new ssh servers servers without prompting for a ssh key
RUN touch /etc/ssh/ssh_config.d/zfs-api.conf && echo "StrictHostKeyChecking accept-new" >> /etc/ssh/ssh_config.d/zfs-api.conf

MAINTAINER Bjorn Smith <bjornsmith@gmail.com>
LABEL maintainer="Bjorn Smith <bjornsmith@gmail.com>"
LABEL version="1.1.10"
LABEL description="A .NET REST API that allows you to inspect and manipulate the zfs file system \
 \
See github repository for more information and source code: https://github.com/bjornbouetsmith/ROOT.Zfs.Api"

# run as the dotnet user
USER dotnet:dotnet

# Tell docker we will be exposing port 5000
EXPOSE 5000/TCP

# run the api in the asp.net environment Docker, which corresponds to the appsettings.Docker.json
ENTRYPOINT ["dotnet","Api.dll","--environment=Docker"]