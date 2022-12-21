namespace BulkInsert.Cascade.Tests.EfCore.TestContext
{
    public class PkIntEntity : IName
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ParentEntity ParentEntity { get; set; }
        public long? ParentEntityId { get; set; }
    }
}