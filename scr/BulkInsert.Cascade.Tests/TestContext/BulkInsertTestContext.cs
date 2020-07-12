using System.Data.Entity;

namespace BulkInsert.Cascade.Tests.TestContext
{
    public class BulkInsertTestContext : DbContext
    {
        public BulkInsertTestContext() : base(
            "Data Source=.;Initial Catalog=BulkInsertTest;Integrated Security=True;MultipleActiveResultSets=true")
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<BulkInsertTestContext>());
        }

        public IDbSet<PkIdentityLong> PkIdentityLongs { get; set; }
        public IDbSet<PkIdentityInt> PkIdentityInts { get; set; }
        public IDbSet<PkIdentityShort> PkIdentityShorts { get; set; }
        public IDbSet<PkIdentityGuid> PkIdentityGuids { get; set; }
        public IDbSet<ParentEntity> ParentEntities { get; set; }

        public IDbSet<ReverseParentEntity> ReverseParentEntities { get; set; }

        public IDbSet<AllColumnsEntity> AllColumnsEntities { get; set; }
    }
}