using System.Data.Entity;
using System.Threading.Tasks;
using BulkInsert.Cascade.Tests.TestContext;
using FluentAssertions;
using Xunit;

namespace BulkInsert.Cascade.Tests
{
    public class MultiLevelCascadeTests : TestBase
    {
        [Fact]
        public async Task AllDefaultsTest()
        {
            var value = new ReverseLevel1Entity
            {
                ReverseLevel2Entity = new ReverseLevel2Entity
                {
                    ReverseLevel3Entity = new ReverseLevel3Entity()
                }
            };
            using var transaction = Context.Database.BeginTransaction();
            await Context.BulkInsertCascade(transaction, new[] {value},
                new CascadeReverse<ReverseLevel1Entity, ReverseLevel2Entity>(o => o.ReverseLevel2Entity,
                    new CascadeReverse<ReverseLevel2Entity, ReverseLevel3Entity>(o => o.ReverseLevel3Entity))
            );
            transaction.Commit();
            var stored = await Context.Set<ReverseLevel1Entity>()
                .Include(o => o.ReverseLevel2Entity)
                .Include(o => o.ReverseLevel2Entity.ReverseLevel3Entity)
                .SingleAsync(o => o.Id == value.Id);
            stored.Should().BeEquivalentTo(value);
        }
    }
}