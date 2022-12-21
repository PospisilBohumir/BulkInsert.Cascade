using System;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.EfCore7.TestContext;
using Xunit;

namespace BulkInsert.Cascade.Tests.EfCore7
{
    public class InheritanceTests : TestBase
    {
        [Fact]
        public async Task InsertBaseTest() => await DefaultTest(new InheritanceBaseEntity {Name = Guid.NewGuid().ToString()});

        [Fact]
        public async Task InsertChild1Test() => await DefaultTest(new InheritanceChild1Entity
        {
            Name = Guid.NewGuid().ToString(),
            Child1 = Guid.NewGuid().ToString(),
        });

        [Fact]
        public async Task InsertChild2Test() => await DefaultTest(new InheritanceChild2Entity
        {
            Name = Guid.NewGuid().ToString(),
            Child1 = Guid.NewGuid().ToString(),
            Child2 = Guid.NewGuid().ToString(),
        });
    }
}