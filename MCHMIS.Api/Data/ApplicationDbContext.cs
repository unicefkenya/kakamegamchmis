using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using MCHMIS.Api.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MCHMIS.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base($"DefaultConnection")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 18000;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = true;
        }

        public DbSet<EnumeratorLocation> EnumeratorLocations { get; set; }

        public DbSet<Enumerator> Enumerators { get; set; }

        public DbSet<County> Counties { get; set; }
        public DbSet<SubCounty> SubCounties { get; set; }
        public DbSet<SubLocation> SubLocations { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Ward> Wards { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<CommunityArea> CommunityAreas { get; set; }
        public DbSet<SystemCode> SystemCodes { get; set; }
        public DbSet<SystemCodeDetail> SystemCodeDetails { get; set; }
        public DbSet<SystemModule> SystemModules { get; set; }
        public DbSet<CVList> CVLists { get; set; }
        public DbSet<CVListDetail> CvListDetails { get; set; }
        public DbSet<HealthFacility> HealthFacilities { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<ApprovalStatus> ApprovalStatus { get; set; }
        public DbSet<HouseholdReg> HouseholdRegs { get; set; }

        public DbSet<HouseholdRegAsset> HouseholdRegAssets { get; set; }
        public DbSet<HouseholdRegCharacteristic> HouseholdRegCharacteristics { get; set; }
        public DbSet<HouseholdRegMember> HouseholdRegMembers { get; set; }
        public DbSet<HouseholdRegMemberDisability> HouseholdRegMemberDisabilities { get; set; }
        public DbSet<HouseholdRegOtherProgramme> HouseholdRegOtherProgrammes { get; set; }
        public DbSet<NotesCategory> NotesCategories { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<Programme> Programmes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<HouseholdReg>().Property(x => x.PMTScore).HasPrecision(10, 18);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}