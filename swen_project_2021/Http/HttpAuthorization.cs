namespace MTCG.Http
{
    public class HttpAuthorization
    {
        public string Type { get; }
        public string Credentials { get; }

        public HttpAuthorization(string type, string credentials)
        {
            Type = type;
            Credentials = credentials;
        }
    }
}
