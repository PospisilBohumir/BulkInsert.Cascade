namespace BulkInsert.Cascade.Tests.EfCore7.TestContext;

public interface IName
{
    public string Name { get; set; }
}

public interface IId
{
    public long Id { get; set; }
}