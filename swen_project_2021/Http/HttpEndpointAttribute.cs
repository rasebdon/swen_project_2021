using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class HttpEndpointAttribute : Attribute
    {
        public string Endpoint { get; }

        public HttpEndpointAttribute(string endpoint)
        {
            Endpoint = endpoint;
        }
    }
}
