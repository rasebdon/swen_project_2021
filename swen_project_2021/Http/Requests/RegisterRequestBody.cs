using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http.Requests
{
    [System.Serializable]
    class RegisterRequestBody
    {
        public string Username { get; }
        public string Password { get; }

        public RegisterRequestBody(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}