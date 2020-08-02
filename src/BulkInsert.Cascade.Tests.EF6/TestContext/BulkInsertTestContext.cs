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

        public IDbSet<PkLongEntity> PkLongEntities { get; set; }
        public IDbSet<PkIntEntity> PkIntEntities { get; set; }
        public IDbSet<PkShortEntity> PkShortEntities { get; set; }
        public IDbSet<PkGuidEntity> PkGuidEntities { get; set; }
        public IDbSet<ParentEntity> ParentEntities { get; set; }
        public IDbSet<ReverseParentEntity> ReverseParentEntities { get; set; }
        public IDbSet<AllColumnTypesEntity> AllColumnTypesEntities { get; set; }
        public IDbSet<ComplexTypeTestEntity> ComplexTypeTestEntities { get; set; }
        public IDbSet<ReverseLevel1Entity> ReverseLevel1Entities { get; set; }
        public IDbSet<ForwardLevel1Entity> ForwardLevel3Entities { get; set; }
        public IDbSet<InheritanceBaseEntity> InheritanceBaseEntities { get; set; }
        public IDbSet<MultipleKeyEntity> MultipleKeyEntities { get; set; }
        public IDbSet<DbGeographyEntity> DbGeographyEntities { get; set; }
        public IDbSet<MissingFkMainEntity> MissingFkMainEntities { get; set; }
    }
}