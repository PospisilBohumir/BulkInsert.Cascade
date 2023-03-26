using System;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.EfCore7.TestContext;
using Xunit;

namespace BulkInsert.Cascade.Tests.EfCore7;

[Collection("Db Context collection")]
public class InheritanceTests
{
    private readonly ContextFixture _contextFixture;

    public InheritanceTests(ContextFixture contextFixture)
    {
        _contextFixture = contextFixture;
    }


    [Fact]
    public async Task InsertBaseTest() => await _contextFixture.DefaultTest(new InheritanceBaseEntity {Name = Guid.NewGuid().ToString()});

    [Fact]
    public async Task InsertChild1Test() => await _contextFixture.DefaultTest(new InheritanceChild1Entity
    {
        Name = Guid.NewGuid().ToString(),
        Child1 = Guid.NewGuid().ToString(),
    });

    [Fact]
    public async Task InsertChild2Test() => await _contextFixture.DefaultTest(new InheritanceChild2Entity
    {
        Name = Guid.NewGuid().ToString(),
        Child1 = Guid.NewGuid().ToString(),
        Child2 = Guid.NewGuid().ToString(),
    });
}