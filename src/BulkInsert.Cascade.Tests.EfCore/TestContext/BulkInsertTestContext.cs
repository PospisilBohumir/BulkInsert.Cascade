using Microsoft.EntityFrameworkCore;

namespace BulkInsert.Cascade.Tests.EfCore.TestContext
{
    public class BulkInsertTestContext : DbContext
    {
        public BulkInsertTestContext()
            : base(new DbContextOptionsBuilder<BulkInsertTestContext>()
                .UseSqlServer("Data Source=.;Initial Catalog=BulkInsertTest.Core;Integrated Security=True;MultipleActiveResultSets=true").Options)
        {
        }



        public DbSet<PkLongEntity> PkLongEntities { get; set; }
        public DbSet<PkIntEntity> PkIntEntities { get; set; }
        public DbSet<PkShortEntity> PkShortEntities { get; set; }
        public DbSet<PkGuidEntity> PkGuidEntities { get; set; }
        public DbSet<ParentEntity> ParentEntities { get; set; }
        public DbSet<ReverseParentEntity> ReverseParentEntities { get; set; }
        public DbSet<AllColumnTypesEntity> AllColumnTypesEntities { get; set; }
        public DbSet<ComplexTypeTestEntity> ComplexTypeTestEntities { get; set; }
        public DbSet<ReverseLevel1Entity> ReverseLevel1Entities { get; set; }
        public DbSet<ForwardLevel1Entity> ForwardLevel3Entities { get; set; }
        public DbSet<InheritanceBaseEntity> InheritanceBaseEntities { get; set; }
        public DbSet<InheritanceChild1Entity> InheritanceChild1Entities { get; set; }
        public DbSet<InheritanceChild2Entity> InheritanceChild2Entities { get; set; }

        //        public DbSet<MultipleKeyEntity> MultipleKeyEntities { get; set; }
        //        public DbSet<DbGeographyEntity> DbGeographyEntities { get; set; }
        //        public DbSet<MissingFkMainEntity> MissingFkMainEntities { get; set; }
    }
}