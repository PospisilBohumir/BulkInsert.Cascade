using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.TestContext;
using FluentAssertions;

namespace BulkInsert.Cascade.Tests
{
    public abstract class TestBase : IDisposable
    {
        protected readonly BulkInsertTestContext Context;

        protected TestBase()
        {
            Context = new BulkInsertTestContext();
        }

        protected async Task DefaultTest<T>(T value) where T : class, IId
        {
            await DefaultTest(value, v => o => o.Id == v.Id);
        }

        protected async Task DefaultTest<T>(T value, Func<T,Expression<Func<T,bool>>> filter) 
            where T : class
        {
            using var transaction = Context.Database.BeginTransaction();
            await Context.BulkInsertWithIdGeneration(transaction, new[] { value });
            transaction.Commit();
            var stored = await Context.Set<T>().SingleAsync(filter(value));
            stored.Should().BeEquivalentTo(value);
        }


        public void Dispose()
        {
            Context.Dispose();
        }
    }
}