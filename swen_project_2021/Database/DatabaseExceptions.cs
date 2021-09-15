using System;

namespace MTCG.Database
{
    class DatabaseException : Exception
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

    class DatabaseConnectionException : DatabaseException
    {
        public DatabaseConnectionException() : base(
            $"There has been an error connecting to the database", 0)
        { }
    }

    class DuplicateEntryException : DatabaseException
    {
        public DuplicateEntryException(string entry) :
            base($"There is already an entry with the key of {entry} in the database!", 1)
        { }
    }

    class InvalidCredentialsException : DatabaseException
    {
        public InvalidCredentialsException(string username) :
            base($"Invalid credentials for user {username} supplied!", 2)
        { }
    }
    class DatabaseInsertException : DatabaseException
    {
        public DatabaseInsertException(string sql) :
            base($"There was an error inserting the entry with the SQL: {sql}!", 3)
        { }
    }
}
