using System;
using MTCG.Controller;
using System.Text.Json;
using MTCG.Requests;

namespace MTCG
{
    static class RestController
    {
        public static string GetResponse(string httpMethod, string urlPath, string requestBody)
        {
            Console.WriteLine($"Request:\nResource: {urlPath}\nMethod: {httpMethod}\nBody: {requestBody}");

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Prepare paths
            string[] path = urlPath.Trim('/').Split("/");

            // Process GET requests
            if (httpMethod == "GET")
            {
                switch (path[0])
                {
                    case "users":
                        return JsonSerializer.Serialize(UserController.GetUser(path[1]), typeof(Models.User), jsonOptions);
                }
            }
            // Process POST requests
            else if (httpMethod == "POST")
            {
                switch (path[0])
                {
                    case "users":
                        var request = JsonSerializer.Deserialize<RegisterRequest>(requestBody);
                        UserController.Register(request.Username, request.Password);

                        return JsonSerializer.Serialize(UserController.GetUser(request.Username));
                }
            }
            else
            {
                throw new Exception("Invalid http method type in request!");
            }

            return "";
        }
    }
}
