using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Authentication
{
    public class StaticUserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
        };

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await Task.Run(() => _users.SingleOrDefault(x => x.Username == username && x.Password == password));

            // return null if user not found

            // authentication successful so return user details without password
            return user;//?.WithoutPassword();
        }
    }
}