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
run apt-get update 

#required to get linux auth working
run apt-get -y install libpam0g-dev 

#required for remote connections to the zfs server
run apt-get -y install ssh

# Create app dir /App and copy publishing output to /App
WORKDIR /App
COPY --from=build-env /Build/out .

# Tell docker we will be exposing port 5000
EXPOSE 5000/TCP

# run the api in the asp.net environment Docker, which corresponds to the appsettings.Docker.json
ENTRYPOINT ["dotnet","Api.dll","--environment=Docker"]