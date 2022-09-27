using Microsoft.Extensions.Configuration;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;

namespace Api.Core
{
    public interface IZfsAccessor
    {
        IZfs Zfs { get; }
    }

    public class ZfsAccessor : IZfsAccessor
    {
        public ZfsAccessor(IRemoteConnection remoteConnection)
        {
            Zfs = new Zfs(remoteConnection.SSHConnection);
        }

        public IZfs Zfs { get; }
    }

    public interface IRemoteConnection
    {
        SSHProcessCall SSHConnection { get; }
    }

    public class RemoteConnection : IRemoteConnection
    {
        public RemoteConnection(IConfiguration config)
        {
            var remoteConfig = config.GetSection("Remote").Get<RemoteHostConfiguration>();
            if (remoteConfig.UseRemoteConnection)
            {
                SSHConnection = new SSHProcessCall(remoteConfig.UserName, remoteConfig.HostName, remoteConfig.UseSudo);
            }
        }


        public SSHProcessCall SSHConnection { get; }
    }

    public class RemoteHostConfiguration
    {
        public bool UseRemoteConnection { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public bool UseSudo { get; set; }
    }
}
