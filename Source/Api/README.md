
# Prequisites

Requires ZFS installed on the server

For Centos/Rockly linux and other RHEL derivatives:

https://openzfs.github.io/openzfs-docs/Getting%20Started/RHEL-based%20distro/index.html

Dotnet needs to be installed
~~~sh
sudo dnf install aspnetcore-runtime-3.1
~~~

Potentially you have to make a symlink to correct libpam, see:
https://github.com/CamW/npam/issues/5

~~~sh
cd /lib64/
sudo ln -s libpam.so.0 ./libpam.so
~~~

or whatever runtime version you want to run

if deploy.sh is to be run via a build pipeline using ssh, then sudo needs to be set up for passwordless.

i.e. in the /etc/sudoers file add the following line:

$USER ALL=(ALL) NOPASSWD: ALL

where $USER is the user that is ssh'ing in to the server to deploy.

An example deployment script that can be used remotely is:

echo "deploying version:$(Build.BuildNumber)"
cd /mnt/zfsapi/zfsapi
tr -d '\r' < /mnt/zfsapi/zfsapi/1.2.3/deploy.sh > /mnt/zfsapi/zfsapi/deploy.sh

sudo sh /mnt/zfsapi/zfsapi/deploy.sh 1.2.3 all

# Docker

You can also run the api in a docker container

## SSH keys
Before building the docker image you need to ensure that whatever private/public key you want to use for ssh connections to the zfs server is present in the directory where the Dockerfile is located.

So before building, copy the keys to the same directory as the Dockerfile

## Building the image

Simply open a shell/powershell/cmd whatever up on the Source folder and run the following command to build the image

```bash
cd Source
docker build -t zfs-api:1.1.10 -f './Dockerfile' --build-arg="SSH_PRIVATE_KEY=id_rsa" --build-arg="SSH_PUBLIC_KEY=id_rsa.pub" .
```

Please note that ssh keys needs to be installed into the docker container, so its possible to run passwordless ssh to the server that runs the zfs filesystem.

The ssh keys needs to be present in the same directory as the `Dockerfile` or else the docker build process will not be able to find them, since the docker build process cannot reach file from outside of the root of the build process which is the location of the `Dockerfile`

## Running the docker container
After having built the image, simply do a:
`docker run zfs-api:1.1.10 -p 5000:5000`
