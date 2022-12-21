using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BulkInsert.Cascade.EfCore;
using BulkInsert.Cascade.Tests.EfCore.TestContext;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BulkInsert.Cascade.Tests.EfCore
{
    public abstract class TestBase : IDisposable
    {
        protected readonly BulkInsertTestContext Context;

        protected TestBase()
        {
            Context = new BulkInsertTestContext();
            Context.Database.EnsureCreated();
        }

        protected async Task DefaultTest<T>(T value) where T : class, IId
        {
            await DefaultTest(value, v => o => o.Id == v.Id);
        }

        protected async Task DefaultTest<T>(T value, Func<T,Expression<Func<T,bool>>> filter) 
            where T : class
        {
            await Context.BulkInsertWithIdGeneration(new[] { value });
            var stored = await Context.Set<T>().SingleAsync(filter(value));
            stored.Should().BeEquivalentTo(value);
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}