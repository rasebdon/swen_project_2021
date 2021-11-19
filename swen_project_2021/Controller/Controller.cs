using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http
{
    public abstract class Controller
    { }

    public interface IHttpGet
    {
        void Get(Dictionary<string, object> parameters);
    }
    public interface IHttpPost
    {
        void Post(string body);
    }
    public interface IHttpDelete
    {
        void Delete(string body);
    }
    public interface IHttpPut
    {
        void Put(string body);
    }
    public interface IHttpPatch
    {
        void Patch(string body);
    }

}
