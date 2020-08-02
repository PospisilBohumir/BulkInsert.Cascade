using System.Collections.Generic;

namespace BulkInsert.Cascade.Tests.EfCore.TestContext
{
    public class ParentEntity
    {
        public long Id { get; set; }
        public virtual ICollection<PkLongEntity> PkIdentityLong { get; set; }
        public virtual ICollection<PkIntEntity> PkIdentityInt { get; set; }
        public virtual ICollection<PkShortEntity> PkIdentityShort { get; set; }
        public virtual ICollection<PkGuidEntity> PkIdentityGuid { get; set; }
    }
}