namespace BulkInsert.Cascade.Tests.EfCore7.TestContext
{
    public class InheritanceBaseEntity : IId
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class InheritanceChild1Entity : InheritanceBaseEntity
    {
        public string Child1 { get; set; }
    }

    public class InheritanceChild2Entity : InheritanceChild1Entity
    {
        public string Child2 { get; set; }
    }
}