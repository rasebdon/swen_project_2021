namespace MTCG.BL.Http
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class HttpEndpointAttribute : Attribute
    {
        public string Endpoint { get; }

        public HttpEndpointAttribute(string endpoint)
        {
            Endpoint = endpoint;
        }
    }
}
