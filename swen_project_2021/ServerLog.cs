using System;

namespace MTCG
{
    static class ServerLog
    {
        public enum OutputFormat
        {
            Standard,
            Warning,
            Error,
            Success
        }

        public static void Print(string msg, OutputFormat format = OutputFormat.Standard)
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
                    tag = "[Error]";
                    break;
                case OutputFormat.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    tag = "[Success]";
                    break;
                case OutputFormat.Standard:
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    tag = "[Server]";
                    break;
            }
            Console.WriteLine("[" + GetCurTime() + "]" + tag + " " + msg);
            // Reset color
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Initialize()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        static string GetCurTime()
        {
            return DateTime.Now.ToString("hh:mm:ss");
        }

        static void PrintColor(string v, ConsoleColor c = ConsoleColor.White)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(v);
            Console.ForegroundColor = old;
        }
    }
}
