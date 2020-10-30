using Microsoft.AspNetCore.Authentication;

namespace Api.Core.Authentication
{
    public class BasicAuthSchemeOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }

        public BasicAuthSchemeOptions()
        {
            
        }
        public BasicAuthSchemeOptions(string realm)
        {
            Realm = realm;
        }
    }
}