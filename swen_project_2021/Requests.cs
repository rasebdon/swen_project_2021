using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Requests
{
    //abstract class Request
    //{
    //    public string RawBody { get; }

    //    protected Request(string rawBody)
    //    {
    //        RawBody = rawBody;
    //    }
    //}

    [System.Serializable]
    class RegisterRequest //: Request
    {
        public string Username { get; }
        public string Password { get; }

        public RegisterRequest(string username, string password) //: base(request)
        {
            Username = username;
            Password = password;
        }
    }
}
