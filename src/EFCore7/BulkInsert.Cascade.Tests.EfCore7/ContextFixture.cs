using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BulkInsert.Cascade.EfCore7;
using BulkInsert.Cascade.Tests.EfCore7.TestContext;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Xunit;

namespace BulkInsert.Cascade.Tests.EfCore7;

public class ContextFixture : IAsyncLifetime
{
    public BulkInsertTestContext Context = default!;
    private Respawner _respawner = default!;

    public async Task DefaultTest<T>(T value) where T : class, IId
    {
        await DefaultTest(value, v => o => o.Id == v.Id);
    }

    public async Task ResetDb()
    {
        var dbConnection = Context.Database.GetDbConnection();
        await _respawner.ResetAsync(dbConnection);
    }

    public async Task DefaultTest<T>(T value, Func<T, Expression<Func<T, bool>>> filter)
        where T : class
    {
        await Context.BulkInsertWithIdGeneration(new[] { value });
        var stored = await Context.Set<T>().SingleAsync(filter(value));
        stored.Should().BeEquivalentTo(value);
    }

    public async Task InitializeAsync()
    {
        Context = new BulkInsertTestContext();
        await Context.Database.EnsureCreatedAsync();
        var dbConnection = Context.Database.GetDbConnection();
        await dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(dbConnection);
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
    }
}

[CollectionDefinition("Db Context collection")]
public class DatabaseCollection : ICollectionFixture<ContextFixture>
{
}