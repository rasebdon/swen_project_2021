using MTCG.BL.Http;
using MTCG.Models;

namespace MTCG.BL
{
    public class RequestWorker : Worker
    {
        public RequestWorker(Server server, ILog log, int id) : base(server, log, id)
        {
        }

        protected override void Run()
        {
            while (_running && _server.HttpClient.IsListening)
            {
                HttpRequest? request;
                try
                {
                    if ((request = _server.HttpClient.GetHttpRequest()) != null)
                    {
                        _log.WriteLine($"RequestWorker{_id} : Recieved an http request!");
                        _server.HttpRequests.Enqueue(request);
                    }
                    else
                    {
                        // Recieved invalid request
                        _log.WriteLine($"RequestWorker{_id} : Invalid request recieved! Continuing to the next request", OutputFormat.Warning);
                    }
                }
                catch (Exception ex)
                {
                    _log.WriteLine(ex.ToString());
                }
            }

            Stop();
        }
    }
}
