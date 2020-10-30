using System.Threading.Tasks;

namespace Api.Core.Authentication
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
    }
}