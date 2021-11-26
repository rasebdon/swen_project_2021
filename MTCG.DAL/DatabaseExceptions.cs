namespace MTCG.DAL
{
    public class DatabaseException : Exception
    {
        /// <summary>
        /// The error code of the exception
        /// </summary>
        public uint ErrorCode { get; }

        public DatabaseException(uint errorCode)
        {
            ErrorCode = errorCode;
        }
        public DatabaseException(string message, uint errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
        public DatabaseException(string message, uint errorCode, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    public class DatabaseConnectionException : DatabaseException
    {
        public DatabaseConnectionException() : base(
            $"There has been an error connecting to the database", 0)
        { }
    }

    public class DuplicateEntryException : DatabaseException
    {
        public DuplicateEntryException(string entry) :
            base($"There is already an entry with the key of {entry} in the database!", 1)
        { }
    }

    public class DatabaseInsertException : DatabaseException
    {
        public DatabaseInsertException(string sql) :
            base($"There was an error inserting the entry with the SQL: {sql}!", 3)
        { }
    }
    public class NoEntryFoundException : DatabaseException
    {
        public NoEntryFoundException(string sql) :
            base($"No entries were found by the given query: {sql}!", 4)
        { }
    }
}
