# Introduction 
A .NET Core based REST API that exposes access to ZFS.

[![.NET CI Build](https://github.com/bjornbouetsmith/ROOT.Zfs.Api/actions/workflows/dotnet-ci-build.yml/badge.svg)](https://github.com/bjornbouetsmith/ROOT.Zfs.Api/actions/workflows/dotnet-ci-build.yml)

# Getting Started
The API can run on both linux and windows - but obviously if you run on windows it will only be able to manage zfs on another host.

This is configured in the appsettings.json - whether or not to use a remote connection or not.

The API itself requires dotnet 6.0 - and if you run on linux - then it expects a working PAM solution, so it can use PAM to authenicate the users calling the api.

Installation is manual for now - but there is a deploy.sh file as an example for linux deployments.

There is also a zfs-api.service unit file, that can be used if you want to run the api as a service on linux.

Latest release is:
https://github.com/bjornbouetsmith/ROOT.Zfs.Api/releases/tag/v1.0.4

## Help needed
If you want to help make this library greater either by 
* Contributing code 
* Writing examples
* Better documentation
* Packages for linux installations or any other OS

or in any other way, feel free to contact [@bjornbouetsmith](https://github.com/bjornbouetsmith).

## NOTE

It is still early days, but if you manage to get the API running on a server, you can access the API documentation via the url:

http://server:port/swagger
