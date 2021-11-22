using MTCG.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MTCG.DAL
{
    public class Database : Singleton<Database>
    {
        public DataTable<User> Users { get; private set; }

        private readonly NpgsqlConnection _databaseConnection;

        public Database(string connectionString)
        {
            _databaseConnection = new NpgsqlConnection(connectionString);

            // Load data tables
            Users = LoadDataTable<User>();

        }

        private DataTable<T> LoadDataTable<T>() where T : DatabaseObject
        {
            // Get table name from model class attribute
            string tableName =
                (new List<Attribute>(typeof(T).GetCustomAttributes())
                .Find(a => a is TableNameAttribute) as TableNameAttribute)
                .TableName;
            if (tableName == null) return null;

            // Prepare lists
            List<T> items = new List<T>();
            List<object> @params = new List<object>();

            // Find constructor
            ConstructorInfo constructor = 
                new List<ConstructorInfo>(typeof(T).GetConstructors())
                .Find(c => c.GetCustomAttribute<TableConstructorAttribute>() != null);
            if (constructor == null) return null;

            // Prepare command
            NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM { tableName };", _databaseConnection);

            // Open connection and start reading rows
            _databaseConnection.Open();
            NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    foreach (var p in constructor.GetParameters())
                    {
                        // Get the values of the rows via the constructor parameters names
                        object value = reader[p.Name];
                        if (p.Name.ToLower() == "id")
                        {
                            value = Guid.Parse(reader[p.Name].ToString());
                        }
                        else Convert.ChangeType(value, p.ParameterType);

                        @params.Add(value);
                    }
                    // Create object
                    T obj = Activator.CreateInstance(typeof(T), @params.ToArray()) as T;

                    items.Add(obj);
                    @params.Clear();
                }
            }

            _databaseConnection.Close();

            return new DataTable<T>(this, items);
        }

        public int ExecuteNonQuery(NpgsqlCommand cmd)
        {
            _databaseConnection.Open();

            cmd.Connection = _databaseConnection;
            int affected = cmd.ExecuteNonQuery();

            _databaseConnection.Close();

            return affected;
        }

        public List<Dictionary<string, object>> ExecuteQuery(NpgsqlCommand cmd)
        {
            List<Dictionary<string, object>> table = new();

            // Prepare command
            cmd.Connection = _databaseConnection;

            // Open connection and start reading rows
            _databaseConnection.Open();
            NpgsqlDataReader reader = cmd.ExecuteReader();
            
            if (reader.HasRows)
            {
                var schema = reader.GetColumnSchema();

                while (reader.Read())
                {
                    table.Add(new Dictionary<string, object>());

                    foreach (var item in schema)
                    {
                        table[^1].Add(item.ColumnName, reader[item.ColumnName]);
                    }
                }
            }

            return table;
        }
    }
}
