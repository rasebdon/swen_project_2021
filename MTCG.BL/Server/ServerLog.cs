using MTCG.Models;

namespace MTCG.BL
{
    public class ServerLog : ILog
    {
        public void Write(string? msg)
        {
            Console.Write(GetFormattedOutput(msg, OutputFormat.Standard));
        }

        public void Write(string? msg, OutputFormat format)
        {
            Console.Write(GetFormattedOutput(msg, format));
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void WriteLine(string? msg)
        {
            Console.WriteLine(GetFormattedOutput(msg, OutputFormat.Standard));
        }

        public void WriteLine(string? msg, OutputFormat format)
        {
            Console.WriteLine(GetFormattedOutput(msg, format));
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static string GetFormattedOutput(string? msg, OutputFormat format)
        {
            string tag;
            switch (format)
            {
                case OutputFormat.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    tag = "[Warning]";
                    break;
                case OutputFormat.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    tag = "[Error]  ";
                    break;
                case OutputFormat.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    tag = "[Success]";
                    break;
                case OutputFormat.Standard:
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    tag = "[Server] ";
                    break;
            }
            return $"[{DateTime.Now:hh:mm:ss}] {tag} {msg}";
        }
    }
}
