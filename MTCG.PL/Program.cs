using MTCG.BL;

namespace MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new(
                System.Net.IPAddress.Loopback,
                10001);
            server.Start(20);
        }
    }
}