using System;

namespace OSAE.General
{
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
