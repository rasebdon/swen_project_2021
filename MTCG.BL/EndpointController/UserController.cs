using CryptSharp;
using MTCG.BL.Http;
using MTCG.BL.Requests;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace MTCG.BL.EndpointController
{
    [HttpEndpoint("/users")]
    public class UserController : Controller, IHttpPost, IHttpGet
    {
        private UserRepository _userRepository;
        private ILog _log;

        public UserController(UserRepository userRepository, ILog log)
        {
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
                    _userRepository.Update(userOld, userNew);
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
            User user = new(Guid.NewGuid(), credentials.Username, hash, 20, 100, 0);
            bool success;
            try
            {
                success = _userRepository.Insert(user);
            }
            catch (DuplicateEntryException e)
            {
                _log.WriteLine(e.ToString(), OutputFormat.Error);
                return new HttpResponse(HttpStatusCode.Conflict);
            }
            catch (Exception e)
            {
                _log.WriteLine(e.ToString(), OutputFormat.Error);
                return new HttpResponse(HttpStatusCode.BadRequest);
            }

            // Remove hash for answer
            user.Hash = "";

            if (success) return new HttpResponse(user.ToJson(), HttpStatusCode.Created, MediaTypeNames.Application.Json);
            else return new HttpResponse(HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        [HttpEndpointArgument]
        public HttpResponse Get(HttpRequest request)
        {
            User? user;

            //HttpEndpointArgumentAttribute? argAttr = MethodBase.GetCurrentMethod()?
            //    .GetCustomAttribute<HttpEndpointArgumentAttribute>();
            //if (argAttr == null || argAttr.Argument == null || argAttr.Argument == "")
            //    return new HttpResponse(HttpStatusCode.InternalServerError);

            //// Get query resource
            //string arg = argAttr.Argument;
            string? arg = request.Argument;

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
                user.SessionToken = "";
                return new HttpResponse(user.ToJson(), HttpStatusCode.OK, MediaTypeNames.Application.Json);
            }
            else return new HttpResponse(HttpStatusCode.BadRequest);
        }
    }
}
