using System;

namespace MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerLog.Initialize(); // Configures the output channel     
            Server server = new("127.0.0.1", 10001);
            server.Start();
        }
    }
}