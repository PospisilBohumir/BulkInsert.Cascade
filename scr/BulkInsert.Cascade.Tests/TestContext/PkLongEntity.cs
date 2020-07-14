namespace BulkInsert.Cascade.Tests.TestContext
{
    public class PkLongEntity : IName
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ParentEntity ParentEntity { get; set; }
        public long? ParentEntityId { get; set; }
    }
}