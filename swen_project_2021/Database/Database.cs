using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MTCG.Database
{
    /// <summary>
    /// Is thrown whenever the database instance is accessed before it was constructed/initialized
    /// </summary>
    class DatabaseNotInitializedException : Exception
    {
        public DatabaseNotInitializedException() : base("The database instance was accessed before it was constructed/initialized!") { }
    }

    /// <summary>
    /// Singleton class for managing the general database communication
    /// </summary>
    class Database
    {
        private NpgsqlConnection Connection { get; }

        public Database(string ip, string database, string username, string password)
        {
            // Connect to postgresql database
            string connectionString = $"Host={ip};Database={database};Username={username};Password={password}";
            Connection = new NpgsqlConnection(connectionString);
        }

        /// <summary>
        /// Opens the database connection
        /// </summary>
        public void OpenConnection()
        {
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

            ServerLog.Print($"Connection to database successful!", ServerLog.OutputFormat.Success);
            ServerLog.Print($"Current database version: {result["version"]}");
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
    }
}
