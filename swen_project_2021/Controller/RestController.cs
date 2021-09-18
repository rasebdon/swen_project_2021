using System;
using MTCG.Controller;
using System.Text.Json;
using MTCG.Http;
using MTCG.Http.Requests;

namespace MTCG
{
    static class RestController
    {
        public static HttpResponse GetResponse(HttpRequest request)
        {
            string resource = request.Url.LocalPath;
            Console.WriteLine($"Request:\nResource: {resource}\nMethod: {request.HttpMethod}\nBody: {request.RequestBody}");

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Prepare paths
            string[] path = resource.Trim('/').Split("/");

            string returnData;

            // Process GET requests
            if (request.HttpMethod == HttpMethod.GET)
            {
                switch (path[0])
                {
                    case "users":
                        returnData = JsonSerializer.Serialize(UserController.GetUser(path[1]), typeof(Models.User), jsonOptions);
                        return new HttpResponse(returnData, HttpStatusCode.OK, "application/json");
                }
            }
            // Process POST requests
            else if (request.HttpMethod == HttpMethod.POST)
            {
                switch (path[0])
                {
                    case "users":
                        RegisterRequestBody registerRequest = JsonSerializer.Deserialize<RegisterRequestBody>(request.RequestBody);
                        
                        try
                        {
                            UserController.Register(registerRequest.Username, registerRequest.Password);
                            returnData = JsonSerializer.Serialize(UserController.GetUser(registerRequest.Username));
                            return new HttpResponse(returnData, HttpStatusCode.Created, "application/json");
                        }
                        catch(Database.DuplicateEntryException)
                        {
                            return new HttpResponse("", HttpStatusCode.Conflict, "");
                        }
                }
            }

            return new HttpResponse("", HttpStatusCode.NotFound, ""); ;
        }
    }
}
