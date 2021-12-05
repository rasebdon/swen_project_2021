using MTCG.BL.EndpointController;
using MTCG.Models;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MTCG.BL.Http
{
    public class RouteEngine
    {
        struct HttpComposedEndpointInfo
        {
            public HttpComposedEndpointInfo(string endpoint, HttpMethod method)
            {
                Endpoint = endpoint;
                Method = method;
            }

            public string Endpoint { get; }
            public HttpMethod Method { get; }
        }
        class HttpComposedEndpointMethod
        {
            public HttpComposedEndpointMethod(MethodInfo method, object @object)
            {
                Method = method;
                Object = @object;
            }

            public MethodInfo Method { get; }
            public object Object { get; }

            public HttpResponse? Invoke(HttpRequest request)
            {
                try
                {
                    return Method.Invoke(Object, new object[] { request }) as HttpResponse;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return new HttpResponse(HttpStatusCode.NotFound);
                }
            }
        }

        private readonly Dictionary<HttpComposedEndpointInfo, HttpComposedEndpointMethod> _httpEndpoints = new();
        private readonly HttpClient _client;
        private readonly ILog _log;

        public RouteEngine(HttpClient client, ILog log)
        {
            _client = client;
            _log = log;
        }

        public void AddController(Controller controller)
        {
            Type type = controller.GetType();

            HttpEndpointAttribute? endpointAttribute = (HttpEndpointAttribute)type.GetCustomAttributes(typeof(HttpEndpointAttribute), false).First();

            if (endpointAttribute != null)
            {
                // Get HttpMethod methods
                foreach (MethodInfo method in type.GetMethods())
                {
                    // Iterate through method attributes
                    foreach (HttpMethodAttribute attribute in method.GetCustomAttributes<HttpMethodAttribute>())
                    {
                        string endpoint = endpointAttribute.Endpoint;

                        // Check if the endpoint is overridden
                        HttpEndpointAttribute? endAttrMethod = method.GetCustomAttribute<HttpEndpointAttribute>();
                        if (endAttrMethod != null)
                        {
                            endpoint = endAttrMethod.Endpoint;
                        }

                        // Check if the method has also the argument attribute
                        HttpEndpointArgumentAttribute? argAttr = method.GetCustomAttribute<HttpEndpointArgumentAttribute>();
                        if (argAttr != null)
                        {
                            endpoint += "/([^/]+)";
                        }

                        _httpEndpoints.Add(
                            new($"^{endpoint}$", attribute.HttpMethod),
                            new(method, controller));

                        _log.WriteLine($"{attribute.HttpMethod,-6} { endpoint }");
                    }
                }
            }
        }
        public void RouteRequest(HttpRequest request)
        {
            HttpComposedEndpointInfo info = new(request.Url.LocalPath, request.HttpMethod);

            foreach (var endpoint in _httpEndpoints)
            {
                Match match = Regex.Match(info.Endpoint, endpoint.Key.Endpoint);
                if (match.Success && endpoint.Key.Method == info.Method)
                {
                    // Set argument of method
                    HttpEndpointArgumentAttribute? argAttr = endpoint.Value.Method.GetCustomAttribute<HttpEndpointArgumentAttribute>();
                    if (argAttr != null && match.Groups[1].Value != "")
                    {
                        //argAttr.Argument = match.Groups[1].Value;
                        request.Argument = match.Groups[1].Value;
                    }

                    try
                    {
                        // Get response and send back request
                        HttpResponse? response = endpoint.Value.Invoke(request);
                        if (response != null)
                            _client.SendHttpResponse(response, request);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        break;
                    }
                    return;
                }
            }
            _client.SendHttpResponse(new HttpResponse(HttpStatusCode.NotFound), request);
        }
    }
}
