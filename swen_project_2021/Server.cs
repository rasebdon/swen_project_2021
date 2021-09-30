using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MTCG.Http;
using MTCG.Controller;

namespace MTCG
{
    public class Server : Singleton<Server>
    {
        public HttpClient HttpClient { get; }

        // Listener tasks
        private List<Task> ListeningTasks { get; set; }
        private readonly int _port;

        public Server(string ip, int port)
        {
            // Configure the output channel
            ServerLog.Initialize();

            _port = port;

            // Set singleton
            Instance = this;

            // Setup http server
            HttpClient = new(port);

            // Initialize database connection
            Database.Instance = new("localhost", "mtcg", "mtcgadmin", "p1s2w3r4");
        }
        /// <summary>
        /// Standard server on port 80
        /// </summary>
        public Server() : this("127.0.0.1", 80) { }

        public void Start(int threads = 0)
        {
            ServerLog.Print("Starting server...");

            try
            {
                Database.Instance.OpenConnection();
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
                // Start listening tasks
                ListeningTasks = new();
                for (int i = 0; i < threads - 1; i++)
                {
                    int taskId = i;
                    var t = Task.Run(() => Listen(taskId));
                    ListeningTasks.Add(t);
                }
            }
            catch (Exception ex)
            {
                ServerLog.Print("Fatal error occured while starting the HTTP server:",
                    ServerLog.OutputFormat.Error);
                ServerLog.Print(ex.ToString(), ServerLog.OutputFormat.Error);
                return;
            }

            // Called on main thread (last listener)
            ListenLast();
        }

        private void ListenLast()
        {
            ServerLog.Print("HTTP server started successfully!", ServerLog.OutputFormat.Success);
            ServerLog.Print($"HTTP server now listening on port {_port} with {ListeningTasks.Count + 1} listeners!");

            Listen(ListeningTasks.Count);
        }

        private void Listen(int taskId)
        {
            while (HttpClient.IsListening)
            {
                // Recieve data
                HttpRequest request = HttpClient.GetHttpRequest();

                ServerLog.Print($"Listener {taskId} recieved an http request!");

                // Recieved invalid request
                if (request == null)
                {
                    ServerLog.Print("Invalid request recieved! Continuing to the next request",
                        ServerLog.OutputFormat.Warning);
                    continue;
                }
                /// Process data
                // No need to process empty get requests
                if (!request.HasEntityBody && request.HttpMethod != HttpMethod.GET)
                {
                    ServerLog.Print("Empty request recieved! Continuing to the next request",
                        ServerLog.OutputFormat.Warning);
                    HttpClient.SendHttpResponse(new HttpResponse(HttpStatusCode.NotFound), request);
                    continue;
                }
                // Get the response
                HttpResponse response;
                try
                {
                     response = RestController.Instance.GetResponse(request);
                }
                catch (Exception ex)
                {
                    ServerLog.Print("There was an error processing the http request:",
                        ServerLog.OutputFormat.Error);
                    ServerLog.Print(ex.ToString(), ServerLog.OutputFormat.Error);
                    HttpClient.SendHttpResponse(new HttpResponse(HttpStatusCode.BadRequest), request);
                    continue;
                }

                HttpClient.SendHttpResponse(response, request);
            }
        }
    }
}
