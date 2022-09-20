using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npam;
using ROOT.Shared.Utils.Serialization;

namespace Api.Core.Authentication
{
    public class LinuxPAMUserService : IUserService
    {
        private readonly ILogger<LinuxPAMUserService> _logger;

        public LinuxPAMUserService(ILogger<LinuxPAMUserService> logger)
        {
            _logger = logger;
        }
        private static string PamService = "password-auth"; // Might be just "password" on non redhat based - so should probably make this based on OS type

        public Task<User> AuthenticateAsync(string username, string password)
        {
            //Console.WriteLine("PAM auth for user:{0}", username);
            
            _logger.Log(LogLevel.Debug, "PAM auth for user:{0}", username);
            if (!NpamUser.Authenticate(PamService, username, password))
            {
                _logger.Log(LogLevel.Debug, "login failure for user:{0}", username);
                return null;
            }


            var groups = NpamUser.GetGroups(username);
            var info = NpamUser.GetAccountInfo(username);

            
            
            var user = new User();
            user.Id = info.UserID;
            user.Username = username;
            user.FirstName = info.RealName.Split(' ').FirstOrDefault();
            user.LastName = info.RealName.Split(' ').LastOrDefault();
            user.Groups = groups.Select(g => new Group { Name = g.GroupName, Id = g.GroupID }).ToList();
            _logger.Log(LogLevel.Trace, user.Dump(new JsonFormatter()));
            Console.WriteLine(user.Dump(new JsonFormatter()));

            return Task.FromResult(user);
        }
    }
}
