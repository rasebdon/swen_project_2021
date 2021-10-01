﻿using MTCG.Controller.Exceptions;
using MTCG.Http;
using MTCG.Http.Requests;
using MTCG.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MTCG.Serialization;

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
                        returnData = UserController.Instance.Select(path[1]).ToJson();
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
                            returnData = UserController.Instance.Select(registerRequest.Username).ToJson();
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
                            if (request.Authorization == null || request.Authorization.Token != "admin-mtcgToken")
                                return new HttpResponse(HttpStatusCode.Forbidden);

                            // Parse package
                            var converter = new CardConverter();
                            Package package = JsonConvert.DeserializeObject<Package>(request.RequestBody, converter);

                            // Insert cards
                            if (!CardController.Instance.Insert(package.Cards))
                                return new HttpResponse(HttpStatusCode.BadRequest);
                            // Insert package
                            if (!PackageController.Instance.Insert(package))
                                return new HttpResponse(HttpStatusCode.BadRequest);

                            return new HttpResponse(package.ToJson(), HttpStatusCode.Created, "application/json");
                        }
                        catch (Exception e)
                        {
                            ServerLog.WriteLine(e.ToString(), ServerLog.OutputFormat.Error);

                            return new HttpResponse(HttpStatusCode.NotFound);
                        }
                    // Transactions
                    case "transactions":
                        try
                        {
                            switch (path[1])
                            {
                                // Buying packages
                                case "packages":
                                    // Get user via auth token
                                    User user = UserController.Instance.Authenticate(request.Authorization);

                                    if (user == null)
                                        return new HttpResponse(HttpStatusCode.BadRequest);

                                    // Parse request body
                                    BuyPackageRequestBody data = BuyPackageRequestBody.FromJson(request.RequestBody);

                                    // Get package via name
                                    Package package = PackageController.Instance.Select(data.PackageName);

                                    // Buy packages
                                    List<CardInstance> drawnCrads = new();
                                    for (int i = 0; i < data.PackageAmount; i++)
                                    {
                                        var d = UserController.Instance.BuyPackage(user.ID, package.ID);

                                        if(d != null)
                                            drawnCrads.AddRange(d);
                                    }
                                    // Serialize the drawn cards
                                    string cardsJson = CardController.Instance.GetDetailedCardsJson(drawnCrads);

                                    return new HttpResponse(cardsJson, HttpStatusCode.Created, "application/json");
                            }
                        }
                        catch(Exception e)
                        {
                            ServerLog.WriteLine(e.ToString(), ServerLog.OutputFormat.Error);
                            break;
                        }
                        break;
                }
            }

            return new HttpResponse(HttpStatusCode.NotFound);
        }
    }
}
