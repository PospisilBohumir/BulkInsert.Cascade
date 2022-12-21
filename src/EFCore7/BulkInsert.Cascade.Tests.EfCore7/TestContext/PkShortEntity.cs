namespace BulkInsert.Cascade.Tests.EfCore7.TestContext
{
    public class PkShortEntity : IName
    {
        public short Id { get; set; }
        public string Name { get; set; }

        public virtual ParentEntity ParentEntity { get; set; }
        public long? ParentEntityId { get; set; }
    }
}