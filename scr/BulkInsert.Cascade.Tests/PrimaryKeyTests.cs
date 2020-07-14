using System;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.TestContext;
using Xunit;

namespace BulkInsert.Cascade.Tests
{
    public class PrimaryKeyTests : TestBase
    {
        [Fact]
        public async Task SimpleBulkInsertPkLongTest() => await PkCheck<PkLongEntity>();

        [Fact]
        public async Task SimpleBulkInsertPkIntTest() => await PkCheck<PkIntEntity>();

        [Fact]
        public async Task SimpleBulkInsertPkShortTest() => await PkCheck<PkShortEntity>();

        [Fact]
        public async Task SimpleBulkInsertPkGuidTest() => await PkCheck<PkGuidEntity>();

        private async Task PkCheck<T>() where T : class, IName, new()
        {
            await DefaultTest(new T {Name = Guid.NewGuid().ToString()},
                v => o => o.Name == v.Name);
        }
    }
}