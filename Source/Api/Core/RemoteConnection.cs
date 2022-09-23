using Microsoft.Extensions.Configuration;
using ROOT.Shared.Utils.OS;

namespace Api.Core
{

    public interface IRemoteConnection
    {
        RemoteProcessCall RemoteProcessCall { get; }
    }

    public class RemoteConnection : IRemoteConnection
    {
        public RemoteConnection(IConfiguration config)
        {
            var remoteConfig = config.GetSection("Remote").Get<RemoteHostConfiguration>();
            if (remoteConfig.UseRemoteConnection)
            {
                RemoteProcessCall = new RemoteProcessCall(remoteConfig.UserName, remoteConfig.HostName, remoteConfig.UseSudo);
            }
        }


        public RemoteProcessCall RemoteProcessCall { get; }
    }

    public class RemoteHostConfiguration
    {
        public bool UseRemoteConnection { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public bool UseSudo { get; set; }
    }
}
