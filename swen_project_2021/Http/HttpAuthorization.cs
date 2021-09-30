namespace MTCG.Http
{
    public class HttpAuthorization
    {
        public string Type { get; }
        public string Token { get; }

        public HttpAuthorization(string type, string credentials)
        {
            Type = type;
            Token = credentials;
        }
    }
}
