using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL
{
    public interface IDatabase
    {
        bool OpenConnection();

        OrderedDictionary[] Select(DbCommand cmd);
        OrderedDictionary SelectSingle(DbCommand cmd);

        int ExecuteNonQuery(DbCommand cmd);

        void Dispose();
    }
}
