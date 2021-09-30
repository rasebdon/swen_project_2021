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

        public static void WriteLine(string msg, OutputFormat format = OutputFormat.Standard)
        {
            string tag;
            switch (format)
            {
                case OutputFormat.Warning:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    tag = "[Warning]";
                    break;
                case OutputFormat.Error:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    tag = "[Error]";
                    break;
                case OutputFormat.Success:
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    tag = "[Success]";
                    break;
                case OutputFormat.Standard:
                default:
                    System.Console.ForegroundColor = ConsoleColor.White;
                    tag = "[Server]";
                    break;
            }
            System.Console.WriteLine("[" + GetCurTime() + "]" + tag + " " + msg);
            // Reset color
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Initialize()
        {
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        static string GetCurTime()
        {
            return DateTime.Now.ToString("hh:mm:ss");
        }

        static void PrintColor(string v, ConsoleColor c = ConsoleColor.White)
        {
            ConsoleColor old = System.Console.ForegroundColor;
            System.Console.ForegroundColor = c;
            System.Console.Write(v);
            System.Console.ForegroundColor = old;
        }
    }
}
