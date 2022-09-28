namespace MCHAPP
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("name=ApplicationDbContext")
        {
        }

        public virtual DbSet<SubCounty> SubCounties { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }
        public virtual DbSet<HealthFacility> HealthFacilities { get; set; }
        public virtual DbSet<ApplicationUser> Users { get; set; }
        public virtual DbSet<HouseholdReg> HouseholdReg { get; set; }
        public virtual DbSet<FingerPrintVerification> FingerPrintVerifications { get; set; }
        public virtual DbSet<MotherClinicVisit> MotherClinicVisits { get; set; }
        public virtual DbSet<Change> Changes { get; set; }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Entity<HouseholdReg>().ToTable("HouseholdRegs");
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            
        }
    }
}