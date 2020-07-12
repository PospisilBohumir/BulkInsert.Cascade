using System;
using BulkInsert.Cascade.Tests.TestContext;

namespace BulkInsert.Cascade.Tests
{
    public abstract class TestBase : IDisposable
    {
        protected readonly BulkInsertTestContext _context;

        protected TestBase()
        {
            _context = new BulkInsertTestContext();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}