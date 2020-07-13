using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.TestContext;
using FluentAssertions;
using Xunit;

namespace BulkInsert.Cascade.Tests
{
    public class ComplexTypeTests : TestBase
    {
        [Fact]
        public async Task SimpleBulkInsertPkLongTest()
        {
            using var transaction = Context.Database.BeginTransaction();
            var entity = new ComplexTypeTestEntity
            {
                ComplexTypeEntity =
                {
                    Field1 = Guid.NewGuid().ToString(),
                    Field2 = 11
                },
                SomeField = Guid.NewGuid().ToString(),
            };
            await Context.BulkInsertWithIdGeneration(transaction, new[] { entity });
            transaction.Commit();
            var anyAsync = await Context.Set<ComplexTypeTestEntity>().Where(o => o.Id == entity.Id)
                .SingleAsync();
            anyAsync.Should().BeEquivalentTo(entity);
        }
    }
}