using System.Data.Entity.Spatial;

namespace BulkInsert.Cascade.Tests.TestContext
{
    public class DbGeographyEntity : IId
    {
        public long Id { get; set; }
        public DbGeography Geography { get; set; }
    }
}