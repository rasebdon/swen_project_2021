using System;
using MTCG.DAL;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
using System.Collections.Generic;

namespace MTCG.DAL
{
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ForeignTableAttribute : Attribute
    {
        public string ForeignTableName { get; }
        public string ForeignRowName { get; }
        public string ThisForeignRowName { get; }

        // N to M over table
        public string ConnectingTableName { get; }
        public string ThisConnectingRowName { get; }
        public string ForeignConnectingRowName { get; }

        public ForeignTableAttribute(Type foreignTableType, string thisForeignRowName, string foreignRowName = "Id")
        {
            // Get table info from type
            ForeignTableName = (foreignTableType.GetCustomAttributes(false)
                .Where(a => a.GetType() == typeof(TableNameAttribute))
                .First() as TableNameAttribute).TableName;
            ThisForeignRowName = thisForeignRowName;
            ForeignRowName = foreignRowName;
        }

        public ForeignTableAttribute(
            Type foreignTableType,
            string thisForeignRowName,
            string foreignRowName,
            string connectingTableName,
            string thisConnectingRowName,
            string foreignConnectingRowName)
            : this(foreignTableType, thisForeignRowName, foreignRowName)
        {
            ConnectingTableName = connectingTableName;
            ThisConnectingRowName = thisConnectingRowName;
            ForeignConnectingRowName = foreignConnectingRowName;
        }
    }
}
