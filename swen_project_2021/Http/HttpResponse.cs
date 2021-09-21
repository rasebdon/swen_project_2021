using MTCG.Http;

namespace MTCG
{
    enum HttpStatusCode
    {
        OK = 200,
        Created = 201,
        BadRequest = 400,
        Forbidden = 401,
        NotFound = 404,
        Conflict = 409
    }

    class HttpResponse
    {
        public string ResponseBody { get; }
        public HttpStatusCode HttpStatusCode { get; }
        public string ContentType { get; }

        public HttpResponse(HttpStatusCode httpStatusCode)
        {
            ContentType = "";
            HttpStatusCode = httpStatusCode;
            ResponseBody = "";
        }

        public HttpResponse(string responseBody, HttpStatusCode httpStatusCode, string contentType)
        {
            ContentType = contentType;
            HttpStatusCode = httpStatusCode;
            ResponseBody = responseBody;
        }
    }
}