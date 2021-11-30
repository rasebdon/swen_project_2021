using MTCG.BL.Http;
using MTCG.Models;
using System.Collections.Concurrent;

namespace MTCG.BL.Services
{
    public class AuthenticationService : Service
    {
        public ConcurrentDictionary<string, User> LoggedInUsers { get; }

        public AuthenticationService()
        {
            LoggedInUsers = new();
        }

        /// <summary>
        /// Returns the user by its auth token
        /// </summary>
        /// <param name="auth">The given auth token of the user</param>
        /// <returns>The user related to the auth token / null if no user was found</returns>
        public User? Authenticate(HttpAuthorization? auth)
        {
            if(auth != null)
            {
                LoggedInUsers.TryGetValue(auth.Token, out User? user);
                return user;
            }
            return null;
        }
    }
}
