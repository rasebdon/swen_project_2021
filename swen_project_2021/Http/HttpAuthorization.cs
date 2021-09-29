using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http
{
    public class HttpAuthorization
    {
        public string Type { get; }
        public string Credentials { get; }

        public HttpAuthorization(string type, string credentials)
        {
            Type = type;
            Credentials = credentials;
        }
    }
}
