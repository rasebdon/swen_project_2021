using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace MTCG.Http
{
    public partial class HttpClient
    {
        class HttpSocket : Socket
        {
            public bool IsListening { get; internal set; }

            private readonly bool _https;
            private readonly int _port;

            public HttpSocket(int port, bool https = false) : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                _port = port;
                _https = https;
                Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
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
                int recievedAmount = conn.Receive(_buffer, 0, ReceiveBufferSize, SocketFlags.None);

                string data = Encoding.UTF8.GetString(_buffer);

                // Split data and construct the request
                StringReader reader = new(data);

                // Get the http header line
                string[] headerData = reader.ReadLine().Split(' ');
                // Parse method
                HttpMethod httpMethod = HttpMethod.GET;
                if (headerData[0] != "HEAD")
                    httpMethod = (HttpMethod)Enum.Parse(typeof(HttpMethod), headerData[0]);

                // Parse url
                string baseUrl = $"{(_https ? "https" : "http")}://{((IPEndPoint)LocalEndPoint).Address.MapToIPv4()}";
                Uri requestUrl = new(
                    new Uri(baseUrl),
                    headerData[1]);
                // Parse http version
                string httpVersion = headerData[2];
                // Find content vars
                bool reading = true;
                int contentLength = 0;
                string contentType = "";
                string requestBody = "";
                HttpAuthorization authorization = null;

                string readString;
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
                responseMessage += $"Content-Type: {response.ContentType}\r\n";
                responseMessage += $"Content-Length: {response.ResponseBody.Length}\r\n";
                responseMessage += $"\r\n";
                responseMessage += response.ResponseBody + "\r\n";
                // Encode and send
                request.Requester.Send(Encoding.UTF8.GetBytes(responseMessage));
                request.Requester.Close();
                Console.WriteLine($"Response sent:\n{responseMessage}");
            }
        }
    }
}
