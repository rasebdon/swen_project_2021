namespace MTCG.Models
{
    public class LogConsoleWrapper : ILog
    {
        public void Write(string? msg)
        {
            Console.Write(msg);
        }

        public void Write(string? msg, OutputFormat format)
        {
            Console.Write(msg);
        }

        public void WriteLine(string? msg)
        {
            Console.WriteLine(msg);
        }

        public void WriteLine(string? msg, OutputFormat format)
        {
            Console.WriteLine(msg);
        }
    }
}
