using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Api.Core.Authentication
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
    }

    public static class AuthExt 
    {
        public static bool UseLinuxAuth(this IConfiguration config)
        {
            return config.GetValue<bool>("UseLinuxAuth");
        }
    }
}