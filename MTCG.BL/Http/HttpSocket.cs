using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MTCG.BL.Http
{
    internal class HttpSocket : Socket
    {
        public bool IsListening { get; internal set; }

        private readonly bool _https;

        public HttpSocket(IPAddress ip, int port, bool https = false) : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            _https = https;
            Bind(new IPEndPoint(ip, port));
            ReceiveBufferSize = 8192;
            _buffer = new byte[ReceiveBufferSize];
        }

        private readonly byte[] _buffer;

        public void StartListening()
        {
            Listen();
            IsListening = true;
        }

        /// <summary>
        /// Waits for an http request and returns the formatted request
        /// </summary>
        /// <returns>The recieved http request</returns>
        public HttpRequest GetRequest()
        {
            var conn = Accept();
            //List<byte> received = new();

            //int offset = 0;
            //int receiveBytes = 0;
            //while ((receiveBytes += conn.Receive(_buffer, offset, ReceiveBufferSize, SocketFlags.None)) > 0)
            //{
            //    // Copy bytes
            //    for (int i = 0; i < receiveBytes; received.Add(_buffer[i++])) ;
            //    offset += receiveBytes;
            //}

            byte[] received = new byte[ReceiveBufferSize];
            conn.Receive(received, 0, received.Length, SocketFlags.None);

            string data = Encoding.UTF8.GetString(received.ToArray());

            // Split data and construct the request
            StringReader reader = new(data);

            // Get the http header line
            string? header = reader.ReadLine();

            if (header == null)
                throw new Exception("Empty http header!");

            string[] headerData = header.Split(' ');

            // Parse method
            HttpMethod httpMethod = HttpMethod.GET;
            if (headerData[0] != "HEAD")
                httpMethod = (HttpMethod)Enum.Parse(typeof(HttpMethod), headerData[0]);

            // Parse url
            string baseUrl = $"{(_https ? "https" : "http")}://{(LocalEndPoint as IPEndPoint)?.Address.MapToIPv4()}";
            Uri requestUrl = new(
                new Uri(baseUrl),
                headerData[1]);
            // Parse http version
            string httpVersion = headerData[2];
            // Find content vars
            bool reading = true;
            string contentType = "";
            string requestBody = "";
            int contentLength;
            HttpAuthorization? authorization = null;

            string? readString;
            while (reading)
            {
                readString = reader.ReadLine();

                if (readString == null)
                {
                    reading = false;
                }
                else if (readString.Contains("Authorization"))
                {
                    string[] authString = readString.Split(' ');
                    authorization = new(authString[1], authString[2]);
                }
                else if (readString.Contains("Content-Type"))
                {
                    contentType = readString.Split(' ')[1];
                }
                else if (readString.Contains("Content-Length"))
                {
                    contentLength = int.Parse(readString.Split(' ')[1]);
                    // Remove line (line break after Content-Length)
                    reader.ReadLine();
                    // Read full body
                    requestBody = reader.ReadToEnd();
                    // Format request body
                    requestBody = requestBody.Remove(contentLength);
                    reading = false;
                }
            }

            return new HttpRequest(requestUrl, httpMethod, contentType, requestBody, conn, httpVersion, authorization);
        }

        /// <summary>
        /// Sends an http response as an answer to an http request
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        public void SendAsync(HttpResponse response, HttpRequest request)
        {
            // Write response http header
            var responseMessage = $"{request.HttpVersion} {(int)response.HttpStatusCode}\r\n";
            responseMessage += $"Date: {DateTime.Now:R}\r\n";
            responseMessage += $"Server: MTCG-Webserver/0.9.15\r\n";
            // Write content
            responseMessage += $"Connection: close\r\n";
            responseMessage += $"Content-Type: {response.ContentType}\r\n";
            responseMessage += $"Content-Length: {response.ResponseBody.Length}\r\n";
            responseMessage += $"\r\n";
            responseMessage += response.ResponseBody + "\r\n";
            // Encode and send
            request.Requester.Send(Encoding.UTF8.GetBytes(responseMessage));
            request.Requester.Close();
            System.Console.WriteLine($"Response sent:\n{responseMessage}");
        }
    }
}
