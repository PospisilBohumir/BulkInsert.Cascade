using System;
using System.Threading.Tasks;
using BulkInsert.Cascade.Ef6;
using BulkInsert.Cascade.Tests.TestContext;
using FluentAssertions;
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

        [Fact]
        public async Task MultipleKeysTest()
        {
            Func<Task> xx = async () =>
                {
                    await Context.BulkInsertWithIdGeneration(new[] {new MultipleKeyEntity()});
                };
            await xx.Should().ThrowAsync<BulkInsertException>();
        }

        private async Task PkCheck<T>() where T : class, IName, new()
        {
            Context.Set<T>().Add(
                new T
                {
                    Name = Guid.NewGuid().ToString()
                });
            await Context.SaveChangesAsync();
            await DefaultTest(new T {Name = Guid.NewGuid().ToString()},
                v => o => o.Name == v.Name);
        }
    }
}