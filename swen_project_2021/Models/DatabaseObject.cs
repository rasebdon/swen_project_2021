using MTCG.DAL;
using System;
using System.Collections.Generic;
using Npgsql;
using System.Reflection;
using System.Text.Json.Serialization;

namespace MTCG.Models
{
    [Serializable]
    [TableName("")]
    public abstract class DatabaseObject : IInsertable, IDeletable, IUpdateable, ICloneable
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        private readonly string _tableName;
        
        protected DatabaseObject(Guid id)
        {
            Id = id;

            // Get table name
            var attribute = GetType().GetCustomAttribute<TableNameAttribute>();
            if (attribute == null)
            {
                throw new Exception("DataObject does not implement the TableNameAttribute!");
            }
            _tableName = attribute.TableName;
        }

        private static Dictionary<string, object> GetPropertyPairs(DatabaseObject from)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var properties = from.GetType().GetProperties();

            foreach (PropertyInfo p in properties)
            {
                dict.Add(p.Name, p.GetValue(from));
            }

            return dict;
        }

        public void Delete(Database database)
        {
            database.ExecuteNonQuery(
                new NpgsqlCommand($"DELETE FROM user WHERE Id=\"{ Id }\";"));
        }
        public void Insert(Database database)
        {
            var dict = GetPropertyPairs(this);
            string valuesSql = "(";
            string namesSql = "(";

            foreach (var p in dict)
            {
                namesSql += $"{ p.Key },";

                // Encapsulate
                if (p.Value is string || p.Value is Guid)
                {
                    valuesSql += $"\"{ p.Value }\",";
                }
                else
                {
                    valuesSql += $"{ p.Value },";
                }
            }
            // Strip last comma
            valuesSql = valuesSql.Remove(valuesSql.Length - 1) + ")";
            namesSql = namesSql.Remove(namesSql.Length - 1) + ")";

            var insertSql = new NpgsqlCommand(
                $"INSERT INTO { _tableName } { namesSql } VALUES { valuesSql };");
            database.ExecuteNonQuery(insertSql);
        }
        public void Update(Database database)
        {
            var dict = GetPropertyPairs(this);
            string sql = $"UPDATE { _tableName } SET ";

            foreach (var p in dict)
            {
                sql += $"{ p.Key }=";

                // Encapsulate
                if (p.Value is string || p.Value is Guid)
                {
                    sql += $"\"{ p.Value }\",";
                }
                else
                {
                    sql += $"{ p.Value },";
                }
            }
            sql = $"{ sql.Remove(sql.Length - 1) } WHERE Id=\"{ Id }\";";
            database.ExecuteNonQuery(new NpgsqlCommand(sql));
        }
    
        public bool Equals(DatabaseObject obj)
        {
            if (obj.GetType() != GetType())
                return false;

            var objP = GetPropertyPairs(obj);

            foreach (var p in GetPropertyPairs(this))
            {
                if (objP[p.Key] != p.Value)
                {
                    if (p.Value is bool && objP[p.Key].ToString() == p.Value.ToString())
                        continue;
                    if (p.Value is Guid && objP[p.Key].ToString() == p.Value.ToString())
                        continue;
                    return false;
                }
            }

            return true;
        }

        public abstract object Clone();
    }
}
