namespace MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new("127.0.0.1", 10001);
            server.Start(20);
        }
    }
}