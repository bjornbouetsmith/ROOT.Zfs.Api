
**Prequisites**

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
tr -d '\r' < /mnt/zfsapi/zfsapi/$(Build.BuildNumber)/deploy.sh > /mnt/zfsapi/zfsapi/deploy.sh

sudo sh /mnt/zfsapi/zfsapi/deploy.sh $(Build.BuildNumber) all
