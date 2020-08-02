using Microsoft.EntityFrameworkCore;

namespace BulkInsert.Cascade.Tests.EfCore.TestContext
{
    [Owned]
    public class ComplexTypeEntity
    {
        public string Field1 { get; set; }

        public long Field2 { get; set; }
    }

    public class ComplexTypeTestEntity : IId
    {
        public long Id { get; set; }

        public ComplexTypeEntity ComplexTypeEntity { get; set; } = new ComplexTypeEntity();

        public string SomeField { get; set; }
    }
}