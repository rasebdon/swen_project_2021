using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http
{
    public enum HttpMethod
    {
        GET,
        POST,
        DELETE,
        PUT
    }

    public class HttpRequest
    {
        public HttpMethod HttpMethod { get; }
        public string RequestBody { get; }
        public bool HasEntityBody { get; }
        public Uri Url { get; }
        public string ContentType { get; }
        public Socket Requester { get; }
        public string HttpVersion { get; }
        public HttpAuthorization Authorization { get; }

        public HttpRequest(Uri url, HttpMethod method, string contentType, string requestBody, Socket requester, string httpVersion, HttpAuthorization authorization)
        {
            HttpVersion = httpVersion;
            HasEntityBody = requestBody.Length > 0;
            HttpMethod = method;
            ContentType = contentType;
            RequestBody = requestBody;
            Url = url;
            Requester = requester;
        }
    }
}
