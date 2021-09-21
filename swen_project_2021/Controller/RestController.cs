using System;
using MTCG.Controller;
using System.Text.Json;
using MTCG.Http;
using MTCG.Http.Requests;
using MTCG.Models;

namespace MTCG
{
    class RestController : Singleton<RestController>
    {
        public HttpResponse GetResponse(HttpRequest request)
        {
            // Get the resource (path)
            string resource = request.Url.LocalPath;
            Console.WriteLine($"Request:\nResource: {resource}\nMethod: {request.HttpMethod}\nBody: {request.RequestBody}");

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Prepare paths / resources
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
                    // Register call
                    case "users":
                        try
                        {
                            // Deserialize body
                            CredentialsRequestBody registerRequest = JsonSerializer.Deserialize<CredentialsRequestBody>(request.RequestBody);
                            // Try to register
                            UserController.Instance.Register(registerRequest.Username, registerRequest.Password);
                            // Answer
                            returnData = JsonSerializer.Serialize(UserController.GetUser(registerRequest.Username));
                            return new HttpResponse(returnData, HttpStatusCode.Created, "application/json");
                        }
                        catch(Database.DuplicateEntryException)
                        {
                            return new HttpResponse(HttpStatusCode.Conflict);
                        }
                        catch(Exception)
                        {
                            return new HttpResponse(HttpStatusCode.BadRequest);
                        }
                    // Login
                    case "sessions":
                        try
                        {
                            // Dezerialize body
                            CredentialsRequestBody loginRequestBody = JsonSerializer.Deserialize<CredentialsRequestBody>(request.RequestBody);
                            // Try to login
                            User user = UserController.Instance.Login(loginRequestBody.Username, loginRequestBody.Password);
                            // Return the session key
                            return new HttpResponse($"{{\"SessionToken\": \"{user.SessionToken}\"}}", HttpStatusCode.OK, "application/json");
                        }
                        catch(Database.InvalidCredentialsException)
                        {
                            return new HttpResponse(HttpStatusCode.Forbidden);
                        }
                        catch(Exception)
                        {
                            return new HttpResponse(HttpStatusCode.BadRequest);
                        }
                    // Create packages
                    case "packages":
                        try
                        {
                            // Check if authorization is admin-token
                            if (request.Authorization.Credentials != "admin-mtcgToken")
                                return new HttpResponse(HttpStatusCode.Forbidden);



                            throw new NotImplementedException();
                        }
                        catch(Exception)
                        {
                            return new HttpResponse(HttpStatusCode.NotFound);
                        }
                }
            }

            return new HttpResponse(HttpStatusCode.NotFound);
        }
    }
}
