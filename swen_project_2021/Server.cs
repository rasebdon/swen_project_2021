using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class Server
    {
        private static Server _instance;
        public static Server Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("Server not constructed!");
                return _instance;
            }
        }

        public Database.Database Database { get; }
        public HttpListener HttpSocket { get; }

        private List<Task> ListeningTasks { get; set; }
        private readonly int _port;

        public Server(string ip, int port)
        {
            _port = port;

            // Set singleton
            _instance = this;

            // Setup http server
            HttpSocket = new();
            HttpSocket.Prefixes.Add($"http://{ip}:{port}/");

            // Initialize database connection
            this.Database = new("localhost", "mtcg", "mtcgadmin", "p1s2w3r4");
        }

        public void Start()
        {
            ServerLog.Print("Starting server...");

            try
            {
                Database.OpenConnection();
            }
            catch(Exception ex)
            {
                ServerLog.Print("Fatal error occured while connecting to the database",
                    ServerLog.OutputFormat.Error);
                ServerLog.Print(ex.ToString(), ServerLog.OutputFormat.Error);
                return;
            }

            // Start HTTP server
            try
            {
                HttpSocket.Start();
            }
            catch (Exception ex)
            {
                ServerLog.Print("Fatal error occured while starting the HTTP server:",
                    ServerLog.OutputFormat.Error);
                ServerLog.Print(ex.ToString(), ServerLog.OutputFormat.Error);
                return;
            }
            ServerLog.Print("HTTP server started successfully!", ServerLog.OutputFormat.Success);
            // Start listening to incoming data
            ListeningTasks = new();
            for (int i = 0; i < 10; i++)
            {
                Task t = new(Listen);
                t.Start();
                ListeningTasks.Add(t);
            }
            Listen();
        }

        private void Listen()
        {
            ServerLog.Print($"HTTP server now listening on port {_port}!");

            while (HttpSocket.IsListening)
            {
                // Recieve data
                HttpListenerContext context = HttpSocket.GetContext();
                HttpListenerRequest request = context.Request;
                /// Process data
                // No need to process empty requests
                if (!request.HasEntityBody)
                {
                    ServerLog.Print("Empty request recieved! Continuing to the next request",
                        ServerLog.OutputFormat.Warning);
                    continue;
                }
                // Read the body data
                string requestBody = "";
                using (Stream body = request.InputStream)
                {
                    var reader = new StreamReader(body, request.ContentEncoding);
                    requestBody = reader.ReadToEnd();
                }
                // Get the response
                string responseString = "";
                try
                {
                    responseString = RestController.GetResponse(
                        request.HttpMethod,
                        request.Url.LocalPath,
                        requestBody);
                }
                catch (Exception ex)
                {
                    ServerLog.Print("There was an error processing the http request:",
                        ServerLog.OutputFormat.Error);
                    ServerLog.Print(ex.ToString(), ServerLog.OutputFormat.Error);
                }

                // Write back
                HttpListenerResponse response = context.Response;
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                response.ContentLength64 = buffer.Length;
                Stream st = response.OutputStream;
                st.Write(buffer, 0, buffer.Length);

                context.Response.Close();
            }
        }
    }
}
