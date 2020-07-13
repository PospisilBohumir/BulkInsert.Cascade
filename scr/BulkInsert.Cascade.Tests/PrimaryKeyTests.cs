using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.TestContext;
using FluentAssertions;
using Xunit;

namespace BulkInsert.Cascade.Tests
{
    public class PrimaryKeyTests : TestBase
    {
        [Fact]
        public async Task SimpleBulkInsertPkLongTest()
        {
            await PkCheck<PkIdentityLong>();
        }

        [Fact]
        public async Task SimpleBulkInsertPkIntTest()
        {
            await PkCheck<PkIdentityInt>();
        }

        [Fact]
        public async Task SimpleBulkInsertPkShortTest()
        {
            await PkCheck<PkIdentityShort>();
        }

        [Fact]
        public async Task SimpleBulkInsertPkGuidTest()
        {
            await PkCheck<PkIdentityGuid>();
        }


        private async Task PkCheck<T>() where T : class,IName, new()
        {
            using var transaction = _context.Database.BeginTransaction();
            var entity = new T {Name = Guid.NewGuid().ToString()};
            await _context.BulkInsertWithIdGeneration(transaction, new[] {entity});
            transaction.Commit();
            var anyAsync = await _context.Set<T>().Where(o => o.Name == entity.Name)
                .SingleAsync();    
            anyAsync.Should().BeEquivalentTo(entity);
        }
    }
}