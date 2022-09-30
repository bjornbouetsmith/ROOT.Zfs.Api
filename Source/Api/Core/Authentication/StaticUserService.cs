using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Authentication
{
    public class StaticUserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        // only intended to be used when debugging on localhost with a ssh connection to a server running zfs
        private List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Test", LastName = "User", Username = "bbs", Password = "password" }
        };

        public Task<User> AuthenticateAsync(string username, string password)
        {
            return Task.FromResult(_users.SingleOrDefault(x => x.Username == username && x.Password == password));
        }
    }
}