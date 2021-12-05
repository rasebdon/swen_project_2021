using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL
{
    public class TransactionObject
    {
        public int ExpectedAffectedRows { get; set; }
        public DbCommand Command { get; set; }

        public TransactionObject(DbCommand command, int expectedAffectedRows)
        {
            Command = command;
            ExpectedAffectedRows = expectedAffectedRows;
        }
    }
}
