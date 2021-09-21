using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http.Requests
{
    [System.Serializable]
    class CredentialsRequestBody
    {
        public string Username { get; }
        public string Password { get; }

        public CredentialsRequestBody(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}