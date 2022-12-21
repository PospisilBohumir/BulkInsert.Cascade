using System;

namespace BulkInsert.Cascade.Tests.EfCore.TestContext
{
    public class ReverseLevel1Entity : IId
    {
        public long Id { get; set; }
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public ReverseLevel2Entity ReverseLevel2Entity { get; set; }

        public long ReverseLevel2EntityId { get; set; }
    }

    public class ReverseLevel2Entity
    {
        public long Id { get; set; }
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public ReverseLevel3Entity ReverseLevel3Entity { get; set; }
        
        public long ReverseLevel3EntityId { get; set; }
    }

    public class ReverseLevel3Entity
    {
        public long Id { get; set; }
        public string Name { get; set; } = Guid.NewGuid().ToString();
    }
}