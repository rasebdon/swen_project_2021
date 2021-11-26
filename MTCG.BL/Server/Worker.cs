using MTCG.Models;

namespace MTCG.BL
{
    public abstract class Worker
    {
        protected bool _running = false;
        protected Task? _task;

        protected readonly int _id;
        protected readonly ILog _log;
        protected readonly Server _server;

        public Worker(Server server, ILog log, int id)
        {
            _id = id;
            _running = false;
            _log = log;
            _task = null;
            _server = server;
        }

        public virtual void Start(bool mt = true)
        {
            if (!_running)
            {
                _running = true;
                if (mt) _task = new TaskFactory().StartNew(Run);
                else Run();
            }
        }

        protected abstract void Run();

        public virtual void Stop()
        {
            _running = false;
            _task?.Dispose();
        }
    }
}
