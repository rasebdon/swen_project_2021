using MTCG.BL.Http;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BL.EndpointController
{
    [HttpEndpoint("/score")]
    public class ScoreController : Controller
    {
        private UserRepository _userRepository;
        private ILog _log;

        public ScoreController(UserRepository userRepository, ILog log)
        {
            _userRepository = userRepository;
            _log = log;
        }

        [HttpGet]
        public HttpResponse GetScoreboard(HttpRequest request)
        {
            // Get all users
            List<User> users = new(_userRepository.GetAll());

            // Sort by elo
            users.Sort(0, users.Count, Comparer<User>.Create((u1, u2) => u2.ELO.CompareTo(u1.ELO)));

            // Remove sensible data
            users.ForEach(u => u.Hash = "");

            // Return top 100 users
            return new HttpResponse(
                JsonConvert.SerializeObject(users),
                HttpStatusCode.OK,
                System.Net.Mime.MediaTypeNames.Application.Json);
        }
    }
}
