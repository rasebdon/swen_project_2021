using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Serialization
{
    class CharStream : MemoryStream
    {
        public void Write(char c)
        {
            Write(c + "");
        }
        public void Write(string s)
        {
            byte[] b = Encoding.UTF8.GetBytes(s);
            Write(b, 0, b.Length);
        }
        public override string ToString()
        {
            return Encoding.UTF8.GetString(this.ToArray());
        }

        public void WriteLine(string s = "")
        {
            Write(s + "\n");
        }
    }
}
