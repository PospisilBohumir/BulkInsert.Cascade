using System;

namespace BulkInsert.Cascade.Ef6
{
    /// <summary>
    ///     Cascade bulk-insert Exception
    /// </summary>
    public class BulkInsertException : Exception
    {
        /// <summary>Creates a new BulkInsertException</summary>
        /// <param name="message">Exception message</param>
        public BulkInsertException(string message) : base(message)
        {
        }
    }
}