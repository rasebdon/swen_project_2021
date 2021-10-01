using MTCG.Controller.Exceptions;
using MTCG.Models;
using MTCG.Serialization;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MTCG.Controller
{
    /// <summary>
    /// Singleton class for managing the general database communication
    /// </summary>
    public class Database : Singleton<Database>
    {
        private NpgsqlConnection Connection { get; }
        public DatabaseConfiguration Configuration { get; }

        public Database() : this(DatabaseConfiguration.DefaultConfiguration) { }
        public Database(DatabaseConfiguration config)
        {
            Configuration = config;
            // Connect to postgresql database
            Connection = new NpgsqlConnection(config.ConnectionString);
            OpenConnection();
        }
        public Database(string ip, string database, string username, string password)
        : this(new DatabaseConfiguration(ip, database, username, password)) { }

        /// <summary>
        /// Opens the database connection
        /// </summary>
        private void OpenConnection()
        {
            if (Connection.State == System.Data.ConnectionState.Open)
                return;

            Connection.Open();

            // Test connection via version select
            var sql = "SELECT * FROM info;";
            NpgsqlCommand cmd = new(sql);

            OrderedDictionary result;
            try
            {
                result = SelectSingle(cmd);
            }
            catch (Exception)
            {
                throw new DatabaseConnectionException();
            }

            ServerLog.WriteLine($"Connection to database successful!", ServerLog.OutputFormat.Success);
            ServerLog.WriteLine($"Current database version: {result["version"]}");
        }

        /// <summary>
        /// Returns the first result of the given sql select query
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <returns>The first row of the result or null if no results where found</returns>
        public OrderedDictionary SelectSingle(NpgsqlCommand cmd)
        {
            // Execute sql
            var results = Select(cmd);

            // Return data
            if (results.Length > 0)
                return results[0];
            return null; // Maybe throw specific exception here
        }

        /// <summary>
        /// Returns all results of the given sql select query
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <returns>All rows of the result or null if no results where found</returns>
        public OrderedDictionary[] Select(NpgsqlCommand cmd)
        {
            // Execute query
            cmd.Connection = this.Connection;
            NpgsqlDataReader rdr = cmd.ExecuteReader();

            if (!rdr.HasRows)
            {
                rdr.Close();
                return Array.Empty<OrderedDictionary>();
            }

            List<OrderedDictionary> rows = new();

            // Run reader over results
            while (rdr.Read())
            {
                // Parse data into dictionaries
                OrderedDictionary row = new();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    row.Add(rdr.GetName(i), rdr[i]);
                }
                rows.Add(row);
            }
            // Close reader and return results
            rdr.Close();
            return rows.ToArray();
        }

        /// <summary>
        /// Executes the given sql statement and returns the number of rows that it affected.
        /// </summary>
        /// <param name="cmd">The command that will be executed</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteNonQuery(NpgsqlCommand cmd)
        {
            cmd.Connection = this.Connection;
            return cmd.ExecuteNonQuery();
        }
    
        //public int Update(string row, string condition, params Tuple<string, object, Type>[] values)
        //{
        //    CharStream s = new();
        //    s.Write($"UPDATE {row} SET ");
        //    for (int i = 0; i < values.Length; i++)
        //    {
        //        s.Write($"{values[i].Item1}=");
        //        s.Write($"@{values[i].Item1} ");
        //        if (i < values.Length - 1)
        //            s.Write(", ");
        //    }
        //    s.Write($"WHERE {condition};");

        //    var sql = s.ToString();
        //    NpgsqlCommand cmd = new(sql)
        //    {
        //        Connection = this.Connection
        //    };

        //    // Add parameters
        //    for (int i = 0; i < values.Length; i++)
        //    {
        //        var value = Convert.ChangeType(values[i].Item2, values[i].Item3);
        //        cmd.Parameters.AddWithValue(values[i].Item1, value);
        //    }

        //    return cmd.ExecuteNonQuery();
        //}
    }
}
