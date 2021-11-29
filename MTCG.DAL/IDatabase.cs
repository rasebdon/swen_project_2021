using System.Collections.Specialized;
using System.Data.Common;

namespace MTCG.DAL
{
    public interface IDatabase
    {
        bool OpenConnection();
        OrderedDictionary SelectSingle(DbCommand cmd);
        OrderedDictionary[] Select(DbCommand cmd);
        int ExecuteNonQuery(DbCommand cmd);
        void Dispose();
    }
}
