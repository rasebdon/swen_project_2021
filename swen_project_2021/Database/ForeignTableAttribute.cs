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
    public class ForeignTable : Attribute
    {
        public string ThisTableName { get; }
        public string ForeignTableName { get; }
        public string ForeignRowName { get; }
        public string ThisForeignRowName { get; }

        // N to M over table
        public string ConnectingTableName { get; }
        public string ThisConnectingRowName { get; }
        public string ForeignConnectingRowName { get; }

        public ForeignTable(Type thisTableType, Type foreignTableType, string thisForeignRowName, string foreignRowName = "Id")
        {
            // Get table info from type
            ThisTableName = (thisTableType.GetCustomAttributes(false)
                .Where(a => a.GetType() == typeof(TableNameAttribute))
                .First() as TableNameAttribute).TableName;
            ForeignTableName = (foreignTableType.GetCustomAttributes(false)
                .Where(a => a.GetType() == typeof(TableNameAttribute))
                .First() as TableNameAttribute).TableName;
            ThisForeignRowName = thisForeignRowName;
            ForeignRowName = foreignRowName;
        }

        public ForeignTable(
            Type thisTableType,
            Type foreignTableType,
            string thisForeignRowName,
            string foreignRowName,
            string connectingTableName,
            string thisConnectingRowName,
            string foreignConnectingRowName)
        {
            // Get table info from type
            ThisTableName = (thisTableType.GetCustomAttributes(false)
                .Where(a => a.GetType() == typeof(TableNameAttribute))
                .First() as TableNameAttribute).TableName;
            ForeignTableName = (foreignTableType.GetCustomAttributes(false)
                .Where(a => a.GetType() == typeof(TableNameAttribute))
                .First() as TableNameAttribute).TableName;
            ThisForeignRowName = thisForeignRowName;
            ForeignRowName = foreignRowName;
            connectingTableName = ConnectingTableName;
            thisConnectingRowName = ThisConnectingRowName;
            foreignConnectingRowName = ForeignConnectingRowName;
        }

        //public string SelectQuery()
        //{
        //    if(nToM)
        //    {


        //        return $"SELECT FROM { ThisTableName }, { ForeignTableName }, { ConnectingTableName }" +
        //            $"WHERE { ThisTableName }.{ ThisForeignRowName }={ ConnectingTableName }.{ ThisConnectingRowName}" +
        //            $"AND { ForeignTableName }.{ ForeignRowName }={ ConnectingTableName }.{ ForeignConnectingRowName };";
        //    }
        //    else return $"SELECT FROM { ThisTableName }, { ForeignTableName } WHERE { ThisTableName }.{ ThisForeignRowName } = {ForeignTableName}.{ ForeignRowName };";
        //}
    }
}
