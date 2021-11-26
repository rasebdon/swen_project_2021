using MTCG.Models;
using Npgsql;
using System.Collections.Specialized;
using System.Data.Common;

namespace MTCG.DAL
{
    /// <summary>
    /// Class for managing the general database communication
    /// </summary>
    public class Database : IDatabase
    {
        public DatabaseConfiguration Configuration { get; }

        private bool _disposed;
        
        private readonly ILog _log;
        private readonly DbConnection _connection; // TODO : Connection pooling
        private readonly object _connectionLock = new();

        public Database() : this(DatabaseConfiguration.DefaultConfiguration, new LogConsoleWrapper()) { }
        public Database(DatabaseConfiguration config, ILog log)
        {
            _disposed = false;
            Configuration = config;
            _log = log;

            // Connect to postgresql database
            _connection = new NpgsqlConnection(config.ConnectionString);
        }
        public Database(string ip, string database, string username, string password, ILog log)
        : this(new DatabaseConfiguration(ip, database, username, password), log) { }

        /// <summary>
        /// Opens the database connection
        /// </summary>
        public bool OpenConnection()
        {
            try
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (_connection.State == System.Data.ConnectionState.Open)
                    return false;

                _connection.Open();

                // Test connection via version select
                var sql = "SELECT * FROM info;";
                DbCommand cmd = new NpgsqlCommand(sql);

                OrderedDictionary result;
                try
                {
                    result = SelectSingle(cmd);
                }
                catch (Exception)
                {
                    throw new DatabaseConnectionException();
                }

                _log.WriteLine($"Connection to database successful!", OutputFormat.Success);
                _log.WriteLine($"Current database version: {result["version"]}");

                return true;
            }
            catch(Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Returns the first result of the given sql select query
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <returns>The first row of the result or an empty collection if no results where found</returns>
        public OrderedDictionary SelectSingle(DbCommand cmd)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            // Execute sql
            OrderedDictionary[] results = Select(cmd);

            // Return data
            if (results.Length > 0)
                return results[0];
            return new OrderedDictionary();
        }

        /// <summary>
        /// Returns all results of the given sql select query
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <returns>All rows of the result or null if no results where found</returns>
        public OrderedDictionary[] Select(DbCommand cmd)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            // Execute query
            cmd.Connection = this._connection;

            List<OrderedDictionary> rows = new();

            lock (_connectionLock)
            {
                DbDataReader rdr = cmd.ExecuteReaderAsync().Result;

                if (!rdr.HasRows)
                {
                    rdr.Close();
                    return Array.Empty<OrderedDictionary>();
                }

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
            }

            return rows.ToArray();
        }

        /// <summary>
        /// Executes the given sql statement and returns the number of rows that it affected.
        /// </summary>
        /// <param name="cmd">The command that will be executed</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteNonQuery(DbCommand cmd)
        {
            if(_disposed) throw new ObjectDisposedException(GetType().FullName);

            cmd.Connection = this._connection;
            int rowsAffected;

            lock (_connectionLock)
            {
                rowsAffected = cmd.ExecuteNonQuery();
            }
            return rowsAffected;
        }

        public void Dispose()
        {
            _disposed = true;
            lock (_connectionLock) _connection.Dispose();
        }
    }
}
