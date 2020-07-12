using System;

namespace BulkInsert.Cascade.Tests.TestContext
{
    public class PkIdentityGuid
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        public virtual ParentEntity ParentEntity { get; set; }
        public long ParentEntityId { get; set; }

    }
}