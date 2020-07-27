using System;

namespace BulkInsert.Cascade.Shared
{
    public class BulkInsertException : Exception
    {
        public BulkInsertException(string message) : base(message)
        {
        }
    }
}