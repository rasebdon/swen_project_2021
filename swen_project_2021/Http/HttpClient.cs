namespace MTCG.Http
{
    public partial class HttpClient
    {
        public bool IsListening { get { return _socket != null && _socket.IsListening; } }

        private readonly HttpSocket _socket;

        public HttpClient(int port, bool https = false)
        {
            _socket = new HttpSocket(port, https);
            _socket.StartListening();
        }

        /// <summary>
        /// Waits for an http request and returns the formatted request
        /// </summary>
        /// <returns>The recieved http request</returns>
        public HttpRequest GetHttpRequest()
        {
            return _socket.GetRequest();
        }
        public void SendHttpResponse(HttpResponse response, HttpRequest request)
        {
            _socket.SendAsync(response, request);
        }
        public void Close()
        {
            _socket.Close();
        }
    }
}
