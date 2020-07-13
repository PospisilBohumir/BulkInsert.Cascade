using System;
using BulkInsert.Cascade.Tests.TestContext;

namespace BulkInsert.Cascade.Tests
{
    public abstract class TestBase : IDisposable
    {
        protected readonly BulkInsertTestContext Context;

        protected TestBase()
        {
            Context = new BulkInsertTestContext();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}