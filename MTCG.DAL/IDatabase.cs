using System.Collections.Specialized;
using System.Data.Common;

namespace MTCG.DAL
{
    public interface IDatabase : IDisposable
    {
        bool OpenConnection();
        OrderedDictionary SelectSingle(DbCommand cmd);
        OrderedDictionary[] Select(DbCommand cmd);
        int ExecuteNonQuery(DbCommand cmd);
        bool ExecuteNonQueryTransaction(IEnumerable<TransactionObject> objects);
    }
}
