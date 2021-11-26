using MTCG.Models;
using System.Collections.Generic;

namespace MTCG.Test
{
    public class TestLog : ILog
    {
        public List<string> Log { get; set; }

        public TestLog()
        {
            Log = new();
        }

        public void Write(string msg)
        {
            Log.Add(msg);
        }

        public void Write(string msg, OutputFormat format)
        {
            Log.Add(msg);
        }

        public void WriteLine(string msg)
        {
            Log.Add(msg);
        }

        public void WriteLine(string msg, OutputFormat format)
        {
            Log.Add(msg);
        }
    }
}
