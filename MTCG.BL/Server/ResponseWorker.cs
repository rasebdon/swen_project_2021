using MTCG.BL.Http;
using MTCG.Models;

namespace MTCG.BL
{
    public class ResponseWorker : Worker
    {
        public ResponseWorker(Server server, ILog log, int id) : base(server, log, id)
        {
        }

        protected override void Run()
        {
            while (_running)
            {
                if (_server.HttpRequests.TryDequeue(out HttpRequest? request))
                {
                    if (request != null)
                    {
                        try
                        {
                            _log.WriteLine($"ResponseWorker{_id} : Processing http request id:{request.Id}");
                            _server.RouteEngine.RouteRequest(request);
                        }
                        catch (Exception ex)
                        {
                            _log.WriteLine($"ResponseWorker{_id} : There was an error processing the http request:", OutputFormat.Error);
                            _log.WriteLine(ex.ToString(), OutputFormat.Error);
                            _server.HttpClient.SendHttpResponse(new HttpResponse(HttpStatusCode.BadRequest), request);
                            continue;
                        }
                    }
                }
            }
        }
    }
}
