using MTCG.Controller.Exceptions;
using MTCG.Http;
using MTCG.Http.Requests;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MTCG.Controller
{
    public class RestController : Singleton<RestController>
    {
        public HttpResponse GetResponse(HttpRequest request)
        {
            // Get the resource (path)
            string resource = request.Url.LocalPath;
            Console.WriteLine($"Request:\nResource: {resource}\nMethod: {request.HttpMethod}\nBody: {request.RequestBody}");

            // Prepare paths / resources
            string[] path = resource.Trim('/').Split("/");

            string returnData;

            // Process GET requests
            if (request.HttpMethod == HttpMethod.GET)
            {
                switch (path[0])
                {
                    case "users":
                        returnData = UserController.Instance.GetUser(path[1]).ToJson();
                        return new HttpResponse(returnData, HttpStatusCode.OK, "application/json");
                    case "packages":
                        returnData = CreateDummyPackage().ToJson();
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
                            CredentialsRequestBody registerRequest = CredentialsRequestBody.FromJson(request.RequestBody);
                            // Try to register
                            UserController.Instance.Register(registerRequest.Username, registerRequest.Password);
                            // Answer
                            returnData = UserController.Instance.GetUser(registerRequest.Username).ToJson();
                            return new HttpResponse(returnData, HttpStatusCode.Created, "application/json");
                        }
                        catch (DuplicateEntryException e)
                        {
                            ServerLog.WriteLine(e.ToString(), ServerLog.OutputFormat.Error);
                            return new HttpResponse(HttpStatusCode.Conflict);
                        }
                        catch (Exception e)
                        {
                            ServerLog.WriteLine(e.ToString(), ServerLog.OutputFormat.Error);
                            return new HttpResponse(HttpStatusCode.BadRequest);
                        }
                    // Login
                    case "sessions":
                        try
                        {
                            // Dezerialize body
                            CredentialsRequestBody loginRequestBody = CredentialsRequestBody.FromJson(request.RequestBody);
                            // Try to login
                            User user = UserController.Instance.Login(loginRequestBody.Username, loginRequestBody.Password);
                            // Return the session key
                            return new HttpResponse($"{{\"SessionToken\": \"{user.SessionToken}\"}}", HttpStatusCode.OK, "application/json");
                        }
                        catch (InvalidCredentialsException)
                        {
                            return new HttpResponse(HttpStatusCode.Forbidden);
                        }
                        catch (Exception)
                        {
                            return new HttpResponse(HttpStatusCode.BadRequest);
                        }
                    // Create packages
                    case "packages":
                        try
                        {
                            // Check if authorization is admin-token
                            if (request.Authorization != null && request.Authorization.Credentials != "admin-mtcgToken")
                                return new HttpResponse(HttpStatusCode.Forbidden);

                            Package p = JsonSerializer.Deserialize<Package>(request.RequestBody);

                            return new HttpResponse(p.ToJson(), HttpStatusCode.Created, "application/json");
                        }
                        catch (Exception)
                        {
                            return new HttpResponse(HttpStatusCode.NotFound);
                        }
                }
            }

            return new HttpResponse(HttpStatusCode.NotFound);
        }

        public static Package CreateDummyPackage()
        {
            List<Card> cards = new();
            // Add some cards
            cards.Add(
                new SpellCard(
                    "Flame Lance",
                    "A fiery lance that not many mages are able to cast",
                    5,
                    Element.Fire,
                    Rarity.Rare));
            cards.Add(
                new MonsterCard(
                    "Lazy Peon",
                    "No work...",
                    3,
                    Element.Normal,
                    Rarity.Common,
                    Race.Orc));
            cards.Add(
                new MonsterCard(
                    "Deathwing",
                    "All shall burn, beneath the shadow of my wings",
                    15,
                    Element.Fire,
                    Rarity.Legendary,
                    Race.Draconid));
            cards.Add(
                new MonsterCard(
                    "Elven Hunter",
                    "Is there something to hunt?",
                    4,
                    Element.Fire,
                    Rarity.Common,
                    Race.Elf));
            cards.Add(
                new SpellCard(
                    "Firestorm",
                    "Fire everything!",
                    10,
                    Element.Fire,
                    Rarity.Epic));

            return new Package("Dummy Package", "This is a dummy package", 5, cards);
        }
    }
}
