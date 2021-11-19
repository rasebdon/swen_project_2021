using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http
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

            public void Invoke(string body)
            {
                try
                {
                    Method.Invoke(Object, new object[] { body });
                }
                catch (TargetParameterCountException)
                {
                    Method.Invoke(Object, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("Method not implemented in class!");
                }
            }

            public void Invoke(Dictionary<string, object> query)
            {
                try
                {
                    Method.Invoke(Object, new object[] { query });
                }
                catch (TargetParameterCountException)
                {
                    Method.Invoke(Object, null);
                }
                catch
                {
                    Console.WriteLine("Method not implemented in class!");
                }
            }
        }

        private readonly Dictionary<HttpComposedEndpointInfo, HttpComposedEndpointMethod> httpEndpoints = new();

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
                    foreach (Attribute attribute in method.GetCustomAttributes())
                    {
                        // Check if parent attribute inherits from httpMethodAttribute
                        if (attribute.GetType().IsSubclassOf(typeof(HttpMethodAttribute)))
                        {
                            httpEndpoints.Add(
                                new(endpointAttribute.Endpoint, ((HttpMethodAttribute)attribute).HttpMethod),
                                new(method, controller));
                        }
                    }
                }
            }
        }
        public void RouteRequest(string endpoint, HttpMethod httpMethod, string query, string body)
        {
            HttpComposedEndpointInfo info = new(endpoint, httpMethod);

            if (httpEndpoints.ContainsKey(info))
            {
                try
                {
                    if (httpMethod == HttpMethod.GET)
                    {
                        Dictionary<string, object> parameters = new();
                        query.Remove(0, 1).Split('&').ToList()
                            .ForEach(pair => parameters.Add(pair.Split('=')[0], pair.Split('=')[1]));
                        httpEndpoints[info].Invoke(parameters);
                    }
                    else
                    {
                        httpEndpoints[info].Invoke(body);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
