using CryptSharp;
using MTCG.BL.EndpointController.Exceptions;
using MTCG.BL.Http;
using MTCG.BL.Requests;
using MTCG.BL.Services;
using MTCG.DAL.Repositories;
using MTCG.Models;
using System.Net.Mime;

namespace MTCG.BL.EndpointController
{
    [HttpEndpoint("/session")]
    public class SessionController : Controller, IHttpPost
    {
        private readonly UserRepository _userRepository;
        private readonly AuthenticationService _authentication;
        private readonly ILog _log;

        public SessionController(
            AuthenticationService authentication,
            UserRepository userRepository,
            ILog log)
        {
            _userRepository = userRepository;
            _authentication = authentication;
            _log = log;
        }

        /// <summary>
        /// Returns the session token of the user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponse Post(HttpRequest request)
        {
            // Try to parse json body
            CredentialsRequestBody? credentials;
            if ((credentials = CredentialsRequestBody.FromJson(request.RequestBody)) == null)
                return new HttpResponse(HttpStatusCode.BadRequest);

            try
            {
                // Check for valid arguments
                if (credentials.Username.Length <= 0 || credentials.Password.Length <= 0)
                    throw new ArgumentException("Username or password input is not valid!");

                User? user = _userRepository.GetByUsername(credentials.Username);

                if (user == null)
                    return new HttpResponse(HttpStatusCode.Forbidden);

                if (_authentication.LoggedInUsers.Contains(user))
                    throw new AlreadyLoggedInException(user);

                bool authenticated = Crypter.CheckPassword(credentials.Password, user.Hash);
                if (!authenticated)
                    throw new InvalidCredentialsException(credentials.Username);

                // Generate auth token and remove hash
                user.Hash = "";
                user.SessionToken = $"{user.Username}-mtcgToken";

                // Add user to the session
                _authentication.LoggedInUsers.Add(user);

                return new HttpResponse(
                    $"{{\"SessionToken\": \"{user.SessionToken}\"}}",
                    HttpStatusCode.OK,
                    MediaTypeNames.Application.Json);
            }
            catch (InvalidCredentialsException)
            {
                return new HttpResponse(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
