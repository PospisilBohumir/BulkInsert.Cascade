using System.Data.Entity;
using System.Threading.Tasks;
using BulkInsert.Cascade.Ef6;
using BulkInsert.Cascade.Shared;
using BulkInsert.Cascade.Tests.TestContext;
using FluentAssertions;
using Xunit;

namespace BulkInsert.Cascade.Tests
{
    public class CascadeTests : TestBase
    {
        [Fact]
        public async Task CascadeTest()
        {
            using var transaction = Context.Database.BeginTransaction();
            var original = new ParentEntity
            {
                PkIdentityGuid = new[] {new PkGuidEntity()},
                PkIdentityInt = new[] {new PkIntEntity()},
                PkIdentityLong = new[] {new PkLongEntity()},
                PkIdentityShort = new[] {new PkShortEntity()}
            };
            await Context.BulkInsertCascade(transaction, new[] {original},
                new Cascade<ParentEntity, PkGuidEntity>(o => o.PkIdentityGuid),
                new Cascade<ParentEntity, PkIntEntity>(o => o.PkIdentityInt),
                new Cascade<ParentEntity, PkShortEntity>(o => o.PkIdentityShort),
                new Cascade<ParentEntity, PkLongEntity>(o => o.PkIdentityLong)
            );
            transaction.Commit();
            var contextParentEntities = await Context.ParentEntities
                .Include(o => o.PkIdentityGuid)
                .Include(o => o.PkIdentityInt)
                .Include(o => o.PkIdentityLong)
                .Include(o => o.PkIdentityShort)
                .SingleAsync();
            contextParentEntities.PkIdentityGuid.Should()
                .BeEquivalentTo(original.PkIdentityGuid, o => o.Excluding(o => o.ParentEntity));
            contextParentEntities.PkIdentityInt.Should()
                .BeEquivalentTo(original.PkIdentityInt, o => o.Excluding(o => o.ParentEntity));
            contextParentEntities.PkIdentityLong.Should()
                .BeEquivalentTo(original.PkIdentityLong, o => o.Excluding(o => o.ParentEntity));
            contextParentEntities.PkIdentityShort.Should()
                .BeEquivalentTo(original.PkIdentityShort, o => o.Excluding(o => o.ParentEntity));
        }

        [Fact]
        public async Task ReverseCascadeTest()
        {
            using var transaction = Context.Database.BeginTransaction();
            var original = new ReverseParentEntity
            {
                PkIdentityGuid = new ReversePkIdentityGuid(),
                PkIdentityInt = new ReversePkIdentityInt(),
                PkIdentityLong = new ReversePkIdentityLong(),
                PkIdentityShort = new ReversePkIdentityShort()
            };
            await Context.BulkInsertCascade(transaction, new[] {original},
                new CascadeReverse<ReverseParentEntity, ReversePkIdentityGuid>(o => o.PkIdentityGuid),
                new CascadeReverse<ReverseParentEntity, ReversePkIdentityInt>(o => o.PkIdentityInt),
                new CascadeReverse<ReverseParentEntity, ReversePkIdentityShort>(o => o.PkIdentityShort),
                new CascadeReverse<ReverseParentEntity, ReversePkIdentityLong>(o => o.PkIdentityLong)
            );
            transaction.Commit();
            var contextParentEntities = await Context.ReverseParentEntities
                .Include(o => o.PkIdentityGuid)
                .Include(o => o.PkIdentityInt)
                .Include(o => o.PkIdentityLong)
                .Include(o => o.PkIdentityShort)
                .SingleAsync();
            contextParentEntities.PkIdentityGuid.Should().BeEquivalentTo(original.PkIdentityGuid);
            contextParentEntities.PkIdentityInt.Should().BeEquivalentTo(original.PkIdentityInt);
            contextParentEntities.PkIdentityLong.Should().BeEquivalentTo(original.PkIdentityLong);
            contextParentEntities.PkIdentityShort.Should().BeEquivalentTo(original.PkIdentityShort);
        }
    }
}