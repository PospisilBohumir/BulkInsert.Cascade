using System;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.EfCore.TestContext;
using Xunit;

namespace BulkInsert.Cascade.Tests.EfCore
{
    public class ComplexTypeTests : TestBase
    {
        [Fact]
        public async Task SimpleBulkInsertPkLongTest()
        {
            await DefaultTest(new ComplexTypeTestEntity
            {
                ComplexTypeEntity = {Field1 = Guid.NewGuid().ToString(), Field2 = 11},
                SomeField = Guid.NewGuid().ToString(),
            });
        }

    }
}