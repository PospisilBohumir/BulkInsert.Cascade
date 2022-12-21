using System.Threading.Tasks;
using BulkInsert.Cascade.EfCore;
using BulkInsert.Cascade.Tests.EfCore.TestContext;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BulkInsert.Cascade.Tests.EfCore
{
    public class CascadeTests : TestBase
    {
        [Fact]
        public async Task CascadeTest()
        {
            var original = new ParentEntity
            {
                PkIdentityGuid = new[] {new PkGuidEntity()},
                PkIdentityInt = new[] {new PkIntEntity()},
                PkIdentityLong = new[] {new PkLongEntity()},
                PkIdentityShort = new[] {new PkShortEntity()}
            };
            await Context.BulkInsertCascade(new[] {original},
                new Cascade<ParentEntity, PkGuidEntity>(o => o.PkIdentityGuid),
                new Cascade<ParentEntity, PkIntEntity>(o => o.PkIdentityInt),
                new Cascade<ParentEntity, PkShortEntity>(o => o.PkIdentityShort),
                new Cascade<ParentEntity, PkLongEntity>(o => o.PkIdentityLong)
            );
            var contextParentEntities = await Context.ParentEntities
                .Include(o => o.PkIdentityGuid)
                .Include(o => o.PkIdentityInt)
                .Include(o => o.PkIdentityLong)
                .Include(o => o.PkIdentityShort)
                .AsNoTracking()
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
            var original = new ReverseParentEntity
            {
                PkIdentityGuid = new ReversePkIdentityGuid(),
                PkIdentityInt = new ReversePkIdentityInt(),
                PkIdentityLong = new ReversePkIdentityLong(),
                PkIdentityShort = new ReversePkIdentityShort()
            };
            await Context.BulkInsertCascade(new[] {original},
                new CascadeReverse<ReverseParentEntity, ReversePkIdentityGuid>(o => o.PkIdentityGuid),
                new CascadeReverse<ReverseParentEntity, ReversePkIdentityInt>(o => o.PkIdentityInt),
                new CascadeReverse<ReverseParentEntity, ReversePkIdentityShort>(o => o.PkIdentityShort),
                new CascadeReverse<ReverseParentEntity, ReversePkIdentityLong>(o => o.PkIdentityLong)
            );

            var contextParentEntities = await Context.ReverseParentEntities
                .Include(o => o.PkIdentityGuid)
                .Include(o => o.PkIdentityInt)
                .Include(o => o.PkIdentityLong)
                .Include(o => o.PkIdentityShort)
                .AsNoTracking()
                .SingleAsync();
            contextParentEntities.PkIdentityGuid.Should().BeEquivalentTo(original.PkIdentityGuid);
            contextParentEntities.PkIdentityInt.Should().BeEquivalentTo(original.PkIdentityInt);
            contextParentEntities.PkIdentityLong.Should().BeEquivalentTo(original.PkIdentityLong);
            contextParentEntities.PkIdentityShort.Should().BeEquivalentTo(original.PkIdentityShort);
        }
/*
        [Fact]
        public async Task MissingFkForwardTest()
        {
            Func<Task> missingFk = async () =>
            {
                await Context.BulkInsertCascade(new[]
                    {
                        new MissingFkMainEntity
                        {
                            MissingFkEntities = new List<MissingFkEntity>
                            {
                                new MissingFkEntity()
                            }
                        }
                    }, new Cascade<MissingFkMainEntity, MissingFkEntity>(o => o.MissingFkEntities));
            };
            await missingFk.Should().ThrowAsync<BulkInsertException>();
        }

        [Fact]
        public async Task MissingFkBackwardTest()
        {
            Func<Task> missingFk = async () =>
            {
                await Context.BulkInsertCascade(new[]
                {
                    new MissingFkEntity
                    {
                        MissingFkMainEntity = new MissingFkMainEntity()
                    }
                }, new CascadeReverse<MissingFkEntity,MissingFkMainEntity>(o => o.MissingFkMainEntity));
            };
            await missingFk.Should().ThrowAsync<BulkInsertException>();
        }
*/
    }
}