using System;

/// <summary>
/// Namespace for OSAE Database and Logging Management
/// </summary>
namespace OSAE.General
{
    /// <summary>
    /// Class used to track current connection status to the database
    /// </summary>
    public class DBConnectionStatus
    {
        public readonly bool Success;
        public readonly Exception CaughtException;

        public DBConnectionStatus(bool success, Exception exception)
        {
            this.Success = success;
            this.CaughtException = exception;
        }
    }
}
