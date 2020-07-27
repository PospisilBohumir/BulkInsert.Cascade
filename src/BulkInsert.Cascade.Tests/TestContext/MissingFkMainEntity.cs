using System.Collections.Generic;

namespace BulkInsert.Cascade.Tests.TestContext
{
    public class MissingFkMainEntity
    {
        public long Id { get; set; }
        public ICollection<MissingFkEntity> MissingFkEntities { get; set; }
    }
    public class MissingFkEntity
    {
        public long Id { get; set; }
        public MissingFkMainEntity MissingFkMainEntity { get; set; }
    }
}