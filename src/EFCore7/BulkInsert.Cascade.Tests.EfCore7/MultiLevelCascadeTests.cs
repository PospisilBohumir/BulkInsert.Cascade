using System.Linq;
using System.Threading.Tasks;
using BulkInsert.Cascade.EfCore7;
using BulkInsert.Cascade.Tests.EfCore7.TestContext;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BulkInsert.Cascade.Tests.EfCore7;

[Collection("Db Context collection")]
public class MultiLevelCascadeTests
{
    private readonly ContextFixture _contextFixture;

    public MultiLevelCascadeTests(ContextFixture contextFixture)
    {
        _contextFixture = contextFixture;
    }


    [Fact]
    public async Task ReverseTest()
    {
        var values =
            Enumerable.Range(0, 3).Select(o => new ReverseLevel1Entity
            {
                ReverseLevel2Entity = new ReverseLevel2Entity
                {
                    ReverseLevel3Entity = new ReverseLevel3Entity()
                }
            }).ToList();
        await _contextFixture.Context.BulkInsertCascade( values,
            new CascadeReverse<ReverseLevel1Entity, ReverseLevel2Entity>(o => o.ReverseLevel2Entity,
                new CascadeReverse<ReverseLevel2Entity, ReverseLevel3Entity>(o => o.ReverseLevel3Entity))
        );
            
        var ids = values.Select(o => o.Id).ToArray();
        var stored = await _contextFixture.Context.Set<ReverseLevel1Entity>()
            .Include(o => o.ReverseLevel2Entity)
            .Include(o => o.ReverseLevel2Entity.ReverseLevel3Entity)
            .Where(o => ids.Contains(o.Id))
            .OrderBy(o => o.Id)
            .ToListAsync();
        stored.Should().BeEquivalentTo(values);
    }

    [Fact]
    public async Task ForwardMultiLevelTest()
    {
        var values =
            Enumerable.Range(0, 3).Select(o => new ForwardLevel1Entity
            {
                ForwardLevel2Entities = Enumerable.Range(0, 3).Select(x =>
                    new ForwardLevel2Entity
                    {
                        ForwardLevel3Entities =
                            Enumerable.Range(0, 3).Select(i => new ForwardLevel3Entity()).ToList()
                    }).ToList()
            }).ToList();
        await _contextFixture.Context.BulkInsertCascade(values,
            new Cascade<ForwardLevel1Entity, ForwardLevel2Entity>(o => o.ForwardLevel2Entities,
                new Cascade<ForwardLevel2Entity, ForwardLevel3Entity>(o => o.ForwardLevel3Entities))
        );
        var ids = values.Select(o => o.Id).ToArray();
        var stored = await _contextFixture.Context.Set<ForwardLevel1Entity>()
            .Include(o => o.ForwardLevel2Entities)
            .Include("ForwardLevel2Entities.ForwardLevel3Entities")
            .Where(o => ids.Contains(o.Id))
            .OrderBy(o => o.Id)
            .ToListAsync();
        stored.Should().BeEquivalentTo(values, o => o.Excluding(o => o.ForwardLevel2Entities));
        stored.SelectMany(o => o.ForwardLevel2Entities)
            .OrderBy(o => o.Id).Should()
            .BeEquivalentTo(values.SelectMany(o => o.ForwardLevel2Entities)
                .OrderBy(o => o.Id), o => o.Excluding(o => o.ForwardLevel3Entities).Excluding(o => o.ForwardLevel1Entity));

        stored.SelectMany(o => o.ForwardLevel2Entities).SelectMany(o => o.ForwardLevel3Entities)
            .OrderBy(o => o.Id).Should()
            .BeEquivalentTo(values.SelectMany(o => o.ForwardLevel2Entities).SelectMany(o => o.ForwardLevel3Entities)
                .OrderBy(o => o.Id), o => o.Excluding(o => o.ForwardLevel2Entity));
    }
}