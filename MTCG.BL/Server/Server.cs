using MTCG.BL.EndpointController;
using MTCG.BL.Http;
using MTCG.BL.Services;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using System.Collections.Concurrent;
using System.Net;
using HttpClient = MTCG.BL.Http.HttpClient;

namespace MTCG.BL
{
    public class Server
    {
        public HttpClient HttpClient { get; }
        public RouteEngine RouteEngine { get; }
        public ConcurrentQueue<HttpRequest> HttpRequests { get; }

        private bool _disposed = true;

        private readonly List<RequestWorker> _requestWorkers;
        private readonly List<ResponseWorker> _responseWorkers;
        private readonly List<BattleWorker> _battleWorkers;

        private readonly int _port;
        private readonly Database _database;
        private readonly ILog _log;

        public Server(IPAddress ip, int port)
        {
            _disposed = false;
            _log = new ServerLog();
            _port = port;

            _requestWorkers = new();
            _responseWorkers = new();
            _battleWorkers = new();
            HttpRequests = new();

            // Setup http server
            HttpClient = new(ip, port);
            RouteEngine = new(HttpClient);

            // Setup database
            _database = new Database();

            // Construct repositories
            UserRepository userRepository = new(_database, _log);
            CardRepository cardRepository = new(_database, _log);
            DeckRepository deckRepository = new(_database, _log);
            StackRepository stackRepository = new(_database, _log);
            PackageRepository packageRepository = new(_database, _log);
            CardInstanceRepository cardInstanceRepository = new(_database, _log);

            // Construct services
            AuthenticationService authenticationService = new();

            // Construct controllers
            UserController userController = new(authenticationService, userRepository, _log);
            SessionController sessionController = new(authenticationService, userRepository, _log);
            CardController cardController = new(authenticationService, cardRepository, stackRepository, _log);
            DeckController deckController = new(authenticationService, deckRepository, stackRepository, _log);
            BattleController battleController = new(authenticationService, deckRepository, _log);
            PackageController packageController = new(authenticationService, userRepository,
                packageRepository, cardInstanceRepository, _log);
            //TradeController tradeController = new(authenticationService, tradeRepository, _log);

            // Bind controllers
            RouteEngine.AddController(userController);
            RouteEngine.AddController(sessionController);
            RouteEngine.AddController(cardController);
            RouteEngine.AddController(deckController);
            RouteEngine.AddController(battleController);
            RouteEngine.AddController(packageController);

            // Setup worker
            var worker = new BattleWorker(this, userRepository, battleController, _log, 0);
            _battleWorkers.Add(worker);
            //worker.Start();
        }
        /// <summary>
        /// Standard server on port 10001
        /// </summary>
        public Server() : this(IPAddress.Loopback, 10001) { }

        public void Start(int threads = 0)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _log.WriteLine("Starting server...");

            try
            {
                // Initialize database connection
                _database.OpenConnection();
            }
            catch (Exception ex)
            {
                _log.WriteLine("Fatal error occured while connecting to the database", OutputFormat.Error);
                _log.WriteLine(ex.ToString(), OutputFormat.Error);
                return;
            }

            // Start Woker
            try
            {
                for (int i = 0; i < threads; i++)
                {
                    var rqWorker = new RequestWorker(this, _log, i);
                    rqWorker.Start(true);
                    _requestWorkers.Add(rqWorker);

                    var rpWorker = new ResponseWorker(this, _log, i);
                    rpWorker.Start(true);
                    _responseWorkers.Add(rpWorker);
                }
            }
            catch (Exception ex)
            {
                _log.WriteLine("Fatal error occured while starting the Http server:", OutputFormat.Error);
                _log.WriteLine(ex.ToString(), OutputFormat.Error);
                return;
            }

            _log.WriteLine("Http server started successfully!", OutputFormat.Success);
            _log.WriteLine($"Http server now listening on port {_port} with {threads} request + response woker!");

            while (!_disposed) 
            {
                if(Console.ReadLine() == "r")
                {
                    _log.WriteLine("Pending requests:");
                    foreach (var r in HttpRequests)
                    {
                        _log.WriteLine(r.ToString());
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            _disposed = true;

            foreach (RequestWorker worker in _requestWorkers)
            {
                worker.Stop();
            }

            // Wait for rest of request processes
            while (HttpRequests.Count > 0) { }

            foreach (ResponseWorker worker in _responseWorkers)
            {
                worker.Stop();
            }

            foreach (BattleWorker worker in _battleWorkers)
            {
                worker.Stop();
            }

            _database.Dispose();
            HttpClient.Close();
        }
    }
}
