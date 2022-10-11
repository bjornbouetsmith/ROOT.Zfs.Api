Name:       root.zfs-api
Version:    1
Release:    1
Summary:    ZFS REST API
License:    MS-PL
Requires: aspnetcore-runtime-6.0, smartmontools, zfs >= 2.1.5-2, util-linux, coreutils

%description
ZFS REST API that exposes a REST interface to manipulate ZFS.

%prep
# we have no source, so nothing here

%pre
echo "zfsapi ALL=(ALL) NOPASSWD: /bin/ls, /bin/lsblk, /sbin/zfs, /sbin/zpool" >/etc/sudoers.d/zfsapi
getent passwd zfsapi >/dev/null 2>&1 || useradd -g zfsapi -r -m -s /bin/bash -U

%build
cat > hello-world.sh <<EOF
#!/usr/bin/bash
echo Hello world
EOF

%install
mkdir -p %{buildroot}/usr/bin/
install -m 755 hello-world.sh %{buildroot}/usr/bin/hello-world.sh

%files
/usr/bin/hello-world.sh

%changelog
# let's skip this for now