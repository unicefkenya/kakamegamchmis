using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Areas.Legacy.Models;
using MCHMIS.Data;
using MCHMIS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reason = MCHMIS.Areas.Legacy.Models.Reason;
using Status = MCHMIS.Areas.Legacy.Models.Status;

namespace MCHMIS.Areas.Legacy.Data
{
    public class LegacyDbContext : IdentityDbContext
    {
        public LegacyDbContext(DbContextOptions<LegacyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            base.OnModelCreating(builder);
            builder.Entity<Dirty>().ToTable("Dirty");
            builder.Entity<Mother>().ToTable("Mother");
            builder.Entity<Status>().ToTable("Status");
            builder.Entity<ApprovalStatus>().ToTable("ApprovalStatus");
        }

        public DbSet<Dirty> Dirty { get; set; }
        public DbSet<Mother> Mother { get; set; }

        public DbSet<Reason> Reasons { get; set; }
        public DbSet<Status> Status { get; set; }

        public DbSet<ApprovalStatus> ApprovalStatus { get; set; }
    }
}