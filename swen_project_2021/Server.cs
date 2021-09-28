using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MTCG.Http;

namespace MTCG
{
    class Server : Singleton<Server>
    {
        public Database.Database Database { get; }

        public HttpSocket HttpSocket { get; }

        // Listener tasks
        private List<Task> ListeningTasks { get; set; }
        private readonly int _port;
        private const ushort LISTENERS_TASKS_AMOUNT = 20;

        public Server(string ip, int port)
        {
            _port = port;

            // Set singleton
            Instance = this;

            // Setup http server
            HttpSocket = new(port);

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
                
                // Start listening tasks
                ListeningTasks = new();
                for (int i = 0; i < LISTENERS_TASKS_AMOUNT - 1; i++)
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
            ServerLog.Print($"HTTP server now listening on port {_port} with {LISTENERS_TASKS_AMOUNT} listeners!");

            Listen(LISTENERS_TASKS_AMOUNT - 1);
        }

        private void Listen(int taskId)
        {
            while (HttpSocket.IsListening)
            {
                // Recieve data
                HttpRequest request = HttpSocket.GetRequest();

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
                    HttpSocket.Send(new HttpResponse("", HttpStatusCode.NotFound, ""), request);
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
                    continue;
                }

                HttpSocket.Send(response, request);
            }
        }
    }
}
