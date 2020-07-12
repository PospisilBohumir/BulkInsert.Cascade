using System.Collections.Generic;

namespace BulkInsert.Cascade.Tests.TestContext
{
    public class ParentEntity
    {
        public long Id { get; set; }
        public virtual ICollection<PkIdentityLong> PkIdentityLong { get; set; }
        public virtual ICollection<PkIdentityInt> PkIdentityInt { get; set; }
        public virtual ICollection<PkIdentityShort> PkIdentityShort { get; set; }
        public virtual ICollection<PkIdentityGuid> PkIdentityGuid { get; set; }
    }
}