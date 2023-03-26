using System;
using System.Collections.Generic;

namespace BulkInsert.Cascade.Tests.EfCore7.TestContext;

public class ForwardLevel1Entity : IId
{
    public long Id { get; set; }
    public string Name { get; set; } = Guid.NewGuid().ToString();
    public ICollection<ForwardLevel2Entity> ForwardLevel2Entities { get; set; }
}

public class ForwardLevel2Entity
{
    public long Id { get; set; }
    public string Name { get; set; } = Guid.NewGuid().ToString();
    public ForwardLevel1Entity ForwardLevel1Entity { get; set; }
    public long ForwardLevel1EntityId { get; set; }
    public ICollection<ForwardLevel3Entity> ForwardLevel3Entities { get; set; }
}

public class ForwardLevel3Entity
{
    public long Id { get; set; }
    public string Name { get; set; } = Guid.NewGuid().ToString();
    public ForwardLevel2Entity ForwardLevel2Entity { get; set; }
    public long ForwardLevel2EntityId { get; set; }
}