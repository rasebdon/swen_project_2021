using System.Net.Sockets;

namespace MTCG.BL.Http
{
    public enum HttpMethod
    {
        GET,
        POST,
        DELETE,
        PUT,
        PATCH
    }

    public class HttpRequest
    {
        private static uint _id = 0;

        public uint Id { get; }
        public HttpMethod HttpMethod { get; }
        public string RequestBody { get; }
        public string? Argument { get; set; }
        public bool HasEntityBody { get; }
        public Uri Url { get; }
        public Dictionary<string, string> Query { get; }
        public string[] Resources { get; }
        public string ContentType { get; }
        public Socket Requester { get; }
        public string HttpVersion { get; }
        public HttpAuthorization? Authorization { get; }

        public HttpRequest(Uri url, HttpMethod method, string contentType, string requestBody, Socket requester, string httpVersion, HttpAuthorization? authorization)
        {
            Id = _id++;
            HttpVersion = httpVersion;
            HasEntityBody = requestBody.Length > 0;
            HttpMethod = method;
            ContentType = contentType;
            RequestBody = requestBody;
            Url = url;
            Requester = requester;
            Authorization = authorization;

            // Get Resources
            Resources = Url.LocalPath.Split('?')[0].Split('/');

            // Get query args
            Query = new();

            List<string> vs = Url.Query.Split('&').ToList();
            foreach (var q in vs)
            {
                if(q == "") continue;

                try
                {
                    Query.Add(q.Split('=')[0], q.Split('=')[1]);
                }
                catch (Exception ex)
                {
                    // Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
