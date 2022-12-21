using System;

namespace BulkInsert.Cascade.Tests.EfCore.TestContext
{
    public class ReverseParentEntity
    {
        public long Id { get; set; }
        public virtual ReversePkIdentityLong PkIdentityLong { get; set; }
        public long PkIdentityLongId { get; set; }
        public virtual ReversePkIdentityInt PkIdentityInt { get; set; }
        public int PkIdentityIntId { get; set; }
        public virtual ReversePkIdentityShort PkIdentityShort { get; set; }
        public short PkIdentityShortId { get; set; }
        public virtual ReversePkIdentityGuid PkIdentityGuid { get; set; }
        public Guid PkIdentityGuidId { get; set; }
    }

    public class ReversePkIdentityLong
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class ReversePkIdentityInt
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ReversePkIdentityShort
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    public class ReversePkIdentityGuid
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}