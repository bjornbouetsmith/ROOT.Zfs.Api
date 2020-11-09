#!/bin/sh

version=$1


function install_service()
{
  ln -s /mnt/zfsapi/zfsapi/current/zfs-api.service /etc/systemd/system/zfs-api.service
  systemctl enable zfs-api
}

function stop()
{
  systemctl stop zfs-api
}

function start()
{
  systemctl start zfs-api
}

function service_exists()
{
  if [[ $(systemctl status|grep 'zfs-api.service') ]]
  then
    return 1
  else
    return 0
  fi
}

function make_symlinks()
{
  cd "/mnt/zfsapi/zfsapi/$version"
  unzip -o ./linux.zip -d run
  cd ..
  rm current
  ln -s "/mnt/zfsapi/zfsapi/$version/run" ./current
}


case $2 in
  start)
    echo "Starting zfs-api"
    start
    ;;
  stop)
    echo "Stopping zfs-zpi"
    stop
    ;;
  check)
    echo "Checking if service exists"
    service_exists
    exists=$?
    if [ $exists -eq 0 ]
    then
      echo "Service does not exist"
    else
      echo "Service exists"
    fi
    ;;
  install)
    echo "Installing zfs-api"
    make_symlinks
    install_service
    ;;
  all)
    echo "Checking and installing zfs-api"
    service_exists
    exists=$?
    if [ $exists -eq 0 ]
    then
      install_service
    fi
    stop
    start
    ;;
esac


