using System.Data.Entity;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.TestContext;
using Xunit;

namespace BulkInsert.Cascade.Tests
{
    public class PrimaryKeyTests : TestBase
    {
        [Fact]
        public async Task SimpleBulkInsertPkLongTest()
        {
            using var transaction = _context.Database.BeginTransaction();
            await _context.BulkInsertWithIdGeneration(transaction, new[]
            {
                new PkIdentityLong()
            });
            transaction.Commit();
            Assert.Equal(1,await _context.PkIdentityLongs.CountAsync());
        }

        [Fact]
        public async Task SimpleBulkInsertPkIntTest()
        {
            using var transaction = _context.Database.BeginTransaction();
            await _context.BulkInsertWithIdGeneration(transaction, new[]
            {
                new PkIdentityInt()
            });
            transaction.Commit();
            Assert.Equal(1, await _context.PkIdentityInts.CountAsync());
        }

        [Fact]
        public async Task SimpleBulkInsertPkShortTest()
        {
            using var transaction = _context.Database.BeginTransaction();
            await _context.BulkInsertWithIdGeneration(transaction, new[]
            {
                new PkIdentityShort()
            });
            transaction.Commit();
            Assert.Equal(1, await _context.PkIdentityShorts.CountAsync());
        }

        [Fact]
        public async Task SimpleBulkInsertPkGuidTest()
        {
            using var transaction = _context.Database.BeginTransaction();
            await _context.BulkInsertWithIdGeneration(transaction, new[]
            {
                new PkIdentityGuid(), 
            });
            transaction.Commit();
            Assert.Equal(1, await _context.PkIdentityGuids.CountAsync());
        }
    }
}
