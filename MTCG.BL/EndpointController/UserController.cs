using CryptSharp;
using MTCG.BL.Http;
using MTCG.BL.Requests;
using MTCG.BL.Services;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace MTCG.BL.EndpointController
{
    [HttpEndpoint("/users")]
    public class UserController : Controller, IHttpPost, IHttpGet
    {
        private UserRepository _userRepository;
        private AuthenticationService _authenticationService;
        private ILog _log;

        public UserController(AuthenticationService authService, UserRepository userRepository, ILog log)
        {
            _authenticationService = authService;
            _userRepository = userRepository;
            _log = log;
        }

        public bool SetAdmin(Guid userID)
        {
            try
            {
                User? userOld = _userRepository.GetById(userID);
                User? userNew = _userRepository.GetById(userID);
                if (userOld != null && userNew != null)
                {
                    userNew.IsAdmin = true;
                    _userRepository.Update(userNew);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        [HttpPost]
        public HttpResponse Post(HttpRequest request)
        {
            // Try to parse json body
            CredentialsRequestBody? credentials;
            if ((credentials = CredentialsRequestBody.FromJson(request.RequestBody)) == null)
                return new HttpResponse(HttpStatusCode.BadRequest);

            // Check username
            if (!Regex.IsMatch(credentials.Username, "[a-zA-Z]{4,24}"))
                throw new ArgumentException("Username is not valid!");
            // Check password
            //if (!Regex.IsMatch(credentials.Password, @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&\-+=()])(?=\S+$).{8,20}$"))
            //    throw new ArgumentException("Password is not valid!");

            // Create password hash
            string hash = Crypter.Blowfish.Crypt(credentials.Password);

            // Insert into db
            User user = new(Guid.NewGuid(), credentials.Username, hash, 20, 100, 0, "Im playing MTCG!", ":-)", 0);
            bool success;
            try
            {
                success = _userRepository.Insert(user);
            }
            catch (Exception e)
            {
                _log.WriteLine(e.ToString(), OutputFormat.Error);
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }

            // Remove hash for answer
            user.Hash = "";

            if (success)
                return new HttpResponse(user.ToJson(), HttpStatusCode.Created, MediaTypeNames.Application.Json);
            else
                return new HttpResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        public HttpResponse GetWithBearerToken(HttpRequest request)
        {
            User? user = _authenticationService.Authenticate(request.Authorization);
            if (user == null)
                return new HttpResponse(HttpStatusCode.Forbidden);
            else
                return new HttpResponse(JsonConvert.SerializeObject(user), HttpStatusCode.OK, MediaTypeNames.Application.Json);
        }

        [HttpGet]
        [HttpEndpoint("/stats")]
        public HttpResponse GetStats(HttpRequest request)
        {
            User? user = _authenticationService.Authenticate(request.Authorization);
            if (user == null)
                return new HttpResponse(HttpStatusCode.Forbidden);

            string stats = $"{{\n'Wins': {user.Wins},\n'PlayedGames': {user.PlayedGames},\n'ELO': {user.ELO}\n}}";
            
            return new HttpResponse(stats, HttpStatusCode.OK, MediaTypeNames.Application.Json);
        }

        [HttpPut]
        public HttpResponse UpdateWithBearerToken(HttpRequest request)
        {
            try
            {
                User? user = _authenticationService.Authenticate(request.Authorization);

                if (user == null)
                    return new HttpResponse(HttpStatusCode.Forbidden);
                
                User? updatedUser = JsonConvert.DeserializeObject<User>(request.RequestBody);

                if (updatedUser == null)
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Assign new vars
                user.Username = updatedUser.Username;

                // Check for new password
                if (updatedUser.Hash != "")
                {
                    // Create password hash
                    user.Hash = Crypter.Blowfish.Crypt(updatedUser.Hash);
                }

                // Update
                if (!_userRepository.Update(user))
                    return new HttpResponse(HttpStatusCode.InternalServerError);

                user.Hash = "";

                return new HttpResponse(JsonConvert.SerializeObject(user), HttpStatusCode.OK, MediaTypeNames.Application.Json);
            }
            catch(Exception ex)
            {
                _log.WriteLine(ex.ToString(), OutputFormat.Error);
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }


        [HttpGet]
        [HttpEndpointArgument]
        public HttpResponse Get(HttpRequest request)
        {
            User? user;
            string? arg = request.Argument;

            if (arg == null)
                return new HttpResponse(HttpStatusCode.BadRequest);

            // Check if it is guid
            if (Guid.TryParse(arg, out Guid id))
            {
                user = _userRepository.GetById(id);
            }
            else
            {
                user = _userRepository.GetByUsername(arg);
            }

            if (user != null)
            {
                user.Hash = "";
                return new HttpResponse(user.ToJson(), HttpStatusCode.OK, MediaTypeNames.Application.Json);
            }
            else return new HttpResponse(HttpStatusCode.BadRequest);
        }

        [HttpDelete]
        [HttpEndpointArgument]
        public HttpResponse Delete(HttpRequest request)
        {
            User? user = _authenticationService.Authenticate(request.Authorization);
            string? arg = request.Argument;

            if (user != null && arg != null && Guid.TryParse(arg, out Guid id))
            {
                if (id == user.ID)
                {
                    bool success = _userRepository.Delete(id);

                    if (success)
                    {
                        if (request.Authorization != null)
                            _authenticationService.LoggedInUsers.TryRemove(request.Authorization.Token, out _);

                        return new HttpResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HttpResponse(HttpStatusCode.InternalServerError);
                    }
                }
                else
                {
                    return new HttpResponse(HttpStatusCode.Forbidden);
                }
            }
            else
            {
                return new HttpResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
