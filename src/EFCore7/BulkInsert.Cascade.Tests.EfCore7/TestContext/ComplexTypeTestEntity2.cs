using Microsoft.EntityFrameworkCore;

namespace BulkInsert.Cascade.Tests.EfCore7.TestContext;

public class ComplexTypeTestEntity2 : IId
{
    public long Id { get; set; }

    public ComplexTypeEntity2 ComplexTypeEntity { get; set; } = new ComplexTypeEntity2();

    public string SomeField { get; set; }
}

[Owned]
public class ComplexTypeEntity2
{
    public string Field1 { get; set; }

    public long Field2 { get; set; }

    public virtual ComplexTypeEntity ComplexTypeEntity { get; set; } = new ComplexTypeEntity();
}