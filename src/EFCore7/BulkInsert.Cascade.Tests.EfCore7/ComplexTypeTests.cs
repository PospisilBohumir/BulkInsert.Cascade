using System;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.EfCore7.TestContext;
using Xunit;

namespace BulkInsert.Cascade.Tests.EfCore7;

[Collection("Db Context collection")]
public class ComplexTypeTests
{
    private readonly ContextFixture _contextFixture;

    public ComplexTypeTests(ContextFixture contextFixture)
    {
        _contextFixture = contextFixture;
    }

    [Fact]
    public async Task SimpleComplexTypeTest()
    {
        await _contextFixture.DefaultTest(new ComplexTypeTestEntity
        {
            ComplexTypeEntity = { Field1 = Guid.NewGuid().ToString(), Field2 = 11 },
            SomeField = Guid.NewGuid().ToString(),
        });
    }

    [Fact]
    public async Task ComplexInComplexTypeTest()
    {
        await _contextFixture.DefaultTest(new ComplexTypeTestEntity2
        {
            ComplexTypeEntity =
            {
                Field1 = Guid.NewGuid().ToString(),
                Field2 = 11,
                ComplexTypeEntity =
                {
                    Field1 = Guid.NewGuid().ToString(),
                    Field2 = 666,
                }
            },
            SomeField = Guid.NewGuid().ToString(),
        });
    }

}
