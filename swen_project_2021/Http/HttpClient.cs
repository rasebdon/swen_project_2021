using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http
{
    public class HttpClient
    {
        public bool IsListening { get { return _socket != null && _socket.IsListening; } }

        private readonly HttpSocket _socket;

        public HttpClient(int port, bool https = false)
        {
            _socket = new HttpSocket(port, https);
            _socket.Start();
        }

        /// <summary>
        /// Waits for an http request and returns the formatted request
        /// </summary>
        /// <returns>The recieved http request</returns>
        public HttpRequest GetRequest()
        {
            return _socket.GetRequest();
        }
        public void SendResponse(HttpResponse response, HttpRequest request)
        {
            _socket.Send(response, request);
        }
        public void Close()
        {
            _socket.Close();
        }
    }
}
