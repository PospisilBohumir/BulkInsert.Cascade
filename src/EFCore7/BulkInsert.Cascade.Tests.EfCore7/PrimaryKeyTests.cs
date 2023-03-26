using System;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.EfCore7.TestContext;
using Xunit;

namespace BulkInsert.Cascade.Tests.EfCore7;

[Collection("Db Context collection")]
public class PrimaryKeyTests
{
    private readonly ContextFixture _contextFixture;

    public PrimaryKeyTests(ContextFixture contextFixture)
    {
        _contextFixture = contextFixture;
    }

    [Fact]
    public async Task SimpleBulkInsertPkLongTest() => await PkCheck<PkLongEntity>();

    [Fact]
    public async Task SimpleBulkInsertPkIntTest() => await PkCheck<PkIntEntity>();

    [Fact]
    public async Task SimpleBulkInsertPkShortTest() => await PkCheck<PkShortEntity>();

    [Fact]
    public async Task SimpleBulkInsertPkGuidTest() => await PkCheck<PkGuidEntity>();
/*
        [Fact]
        public async Task MultipleKeysTest()
        {
            Func<Task> xx = async () =>
                {
                    await Context.BulkInsertWithIdGeneration(new[] {new MultipleKeyEntity()});
                };
            await xx.Should().ThrowAsync<BulkInsertException>();
        }
*/
    private async Task PkCheck<T>() where T : class, IName, new()
    {
        _contextFixture.Context.Set<T>().Add(
            new T
            {
                Name = Guid.NewGuid().ToString()
            });
        await _contextFixture.Context.SaveChangesAsync();
        await _contextFixture.DefaultTest(new T {Name = Guid.NewGuid().ToString()},
            v => o => o.Name == v.Name);
    }
}