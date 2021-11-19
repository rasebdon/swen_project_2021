using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public abstract class HttpMethodAttribute : Attribute
    {
        public HttpMethod HttpMethod { get; }

        protected HttpMethodAttribute(HttpMethod httpMethod)
        {
            HttpMethod = httpMethod;
        }
    }

    public class HttpGetAttribute : HttpMethodAttribute
    {       
        public HttpGetAttribute() : base(HttpMethod.GET) { }
    }

    public class HttpPostAttribute : HttpMethodAttribute
    {
        public HttpPostAttribute() : base(HttpMethod.POST) { }
    }

    public class HttpPutAttribute : HttpMethodAttribute
    {
        public HttpPutAttribute() : base(HttpMethod.PUT) { }
    }

    public class HttpDeleteAttribute : HttpMethodAttribute
    {
        public HttpDeleteAttribute() : base(HttpMethod.DELETE) { }
    }

    public class HttpPatchAttribute : HttpMethodAttribute
    {
        public HttpPatchAttribute() : base(HttpMethod.PATCH) { }
    }
}
