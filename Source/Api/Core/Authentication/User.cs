using System.Collections.Generic;

namespace Api.Core.Authentication
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Group> Groups { get; set; }
    }
}