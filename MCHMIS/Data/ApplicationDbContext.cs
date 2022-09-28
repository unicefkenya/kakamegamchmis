using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Models;
using MCHMIS.ViewModels;

namespace MCHMIS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            // builder.Entity<PMTCutOff>().HasOne(t => t.Locality).WithOne().OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HouseholdReg>()
                .HasIndex(i => new
                {
                    i.CreatedById,
                    i.CommunityAreaId,
                    i.VillageId,
                    i.StatusId,
                    i.HealthFacilityId,
                    i.IPRSExceptionId,
                    i.MotherId,
                    i.WardId
                });
            builder.Entity<Payment>()
                .HasIndex(i => new
                {
                    i.PayrollId,
                    i.CycleId,
                    i.BeneficiaryId,
                    i.StatusId,
                    i.HealthFacilityId,
                    i.VillageId,
                    i.WardId,
                    i.SubCountyId,
                });
            builder.Entity<Registration>().ToTable("Registration");
            builder.Entity<InstallationSetup>().ToTable("InstallationSetup");
            builder.Entity<AuditTrail>().ToTable("AuditTrail");
            builder.Entity<CVList>().ToTable("CVLists");
            builder.Entity<Notes>().ToTable("Notes");
            builder.Entity<CVListDetail>().ToTable("CVListDetails");
            builder.Entity<Status>().ToTable("Status");
            builder.Entity<ApprovalStatus>().ToTable("ApprovalStatus");
            builder.Entity<SMS>().ToTable("SMS");
            builder.Entity<TempTable>().ToTable("TempTable");

            builder.Entity<PMT_Rural>().ToTable("PMT_Rural");
            builder.Entity<PMT_Urban>().ToTable("PMT_Urban");
            builder.Entity<Scoring_Coef>().ToTable("Scoring_Coef");
            builder.Entity<PMT_Household>().ToTable("PMT_Household");
            builder.Entity<MpesaAuthorizationCode>().ToTable("MpesaAuthorizationCode");
            // builder.Entity<MPesaFeedBack>().ToTable("MPesaFeedBack");

            builder.Entity<CaseManagement>().ToTable("CaseManagement");
            builder.Entity<DrugsAdministered>().ToTable("DrugsAdministered");
            builder.Entity<PregnancyData>().ToTable("PregnancyData");
            builder.Entity<ChildFeedingInformation>().ToTable("ChildFeedingInformation");
            builder.Entity<CaseManagementStatus>().ToTable("CaseManagementStatus");
            builder.Entity<ComplaintStatus>().ToTable("ComplaintStatus");

            builder.Entity<RegistrationSummary>().ToTable("RegistrationSummary");
            builder.Entity<CommunityValidationSummary>().ToTable("CommunityValidationSummary");
            builder.Entity<EnrolmentSummary>().ToTable("EnrolmentSummary");
            builder.Entity<PaymentsSummary>().ToTable("PaymentsSummary");
            builder.Entity<ComplaintsSummary>().ToTable("ComplaintsSummary");
            builder.Entity<CaseManagementSummary>().ToTable("CaseManagementSummary");

            //builder.Entity<PMTCutOff>()
            //    .HasKey(c => new { c.PMTModelId, c.LocalityId });
            builder.Entity<PMTWeight>()
                .HasKey(c => new { c.ConstantId, c.LocalityId });

            builder.Entity<RoleProfile>()
                .HasKey(c => new { c.RoleId, c.TaskId });
            // builder.Entity<HouseholdReg>().HasOne(u => u.HouseholdRegCharacteristic);

            builder.Entity<HouseholdRegMember>().HasOne(u => u.Household).WithMany(i => i.HouseholdRegMembers);
            builder.Entity<HouseholdRegAsset>()
                .HasKey(c => new { c.HouseholdId, c.AssetId });
            builder.Entity<HouseholdRegMemberDisability>()
                .HasKey(c => new { c.HouseholdRegMemberId, c.DisabilityId });
            builder.Entity<RoleProfile>()
                .HasKey(c => new { c.TaskId, c.RoleId });
            builder.Entity<PMTWeight>()
                .HasKey(c => new { c.ConstantId, c.LocalityId });
            builder.Entity<HouseholdRegOtherProgramme>()
                .HasKey(c => new { c.HouseholdId, c.OtherProgrammeId });

            builder.Entity<MotherPreventiveService>()
                .HasKey(c => new { c.PregnancyDataId, c.PreventiveServiceId });
            builder.Entity<PostNatalMilestone>()
                .HasKey(c => new { c.PostNatalExaminationDetailId, c.KeyMilestoneId });
            builder.Entity<PMTCTIntervention>()
                .HasKey(c => new { c.PMTCTServiceId, c.InterventionId });

            builder.Entity<ChildDevelopmentMilestone>()
                .HasKey(c => new { c.ChildId, c.DevelopmentMilestoneId });
            //builder.Entity<PostNatalExaminationDetail>()
            //    .HasKey(c => new { c.ChildId, c.PostNatalExaminationId});

            builder.Entity<FingerPrintVerification>()
                .HasKey(c => new { c.Id, c.VisitDate });

            builder.Entity<HouseholdReg>(eb =>
            {
                eb.Property(b => b.PMTScore).HasColumnType("decimal(18, 10)");
            });
           
            builder.Query<ViewReportsDisbursements>().ToView("ViewReportsDisbursements");
        }
        public DbSet<County> Counties { get; set; }
        public DbSet<SubCounty> SubCounties { get; set; }

        public DbSet<Ward> Wards { get; set; }
        public DbSet<SystemTask> SystemTasks { get; set; }

        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<MailSetting> MailSettings { get; set; }
        public DbSet<AuditTrail> AuditTrail { get; set; }
        public DbSet<RoleProfile> RoleProfiles { get; set; }
        public DbSet<HealthFacility> HealthFacilities { get; set; }

        public DbSet<SystemCode> SystemCodes { get; set; }
        public DbSet<SystemCodeDetail> SystemCodeDetails { get; set; }

        public DbSet<HouseholdReg> HouseholdRegs { get; set; }
        public DbSet<HouseholdRegAsset> HouseholdRegAssets { get; set; }
        public DbSet<HouseholdRegCharacteristic> HouseholdRegCharacteristics { get; set; }
        public DbSet<HouseholdRegMember> HouseholdRegMembers { get; set; }
        public DbSet<HouseholdRegMemberDisability> HouseholdRegMemberDisabilities { get; set; }
        public DbSet<HouseholdRegOtherProgramme> HouseholdRegOtherProgrammes { get; set; }
        public DbSet<PMTCutOff> PMTCutOffs { get; set; }

        public DbSet<PMTScore> PMTScores { get; set; }
        public DbSet<PMTWeight> PMTWeights { get; set; }

        public DbSet<Programme> Programmes { get; set; }
        public DbSet<SubLocation> SubLocations { get; set; }

        public DbSet<GeoMaster> GeoMaster { get; set; }
        public DbSet<Constituency> Constituencies { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<CommunityArea> CommunityAreas { get; set; }

        public DbSet<CVList> CVLists { get; set; }
        public DbSet<CVListDetail> CvListDetails { get; set; }

        public DbSet<Status> Status { get; set; }
        public DbSet<Reason> Reasons { get; set; }
        public DbSet<ApprovalStatus> ApprovalStatus { get; set; }

        public DbSet<NotesCategory> NotesCategories { get; set; }
        public DbSet<Notes> Notes { get; set; }

        //PMT Tables
        public DbSet<PMT_Rural> PMT_Rural { get; set; }

        public DbSet<PMT_Urban> PMT_Urban { get; set; }
        public DbSet<Scoring_Coef> Scoring_Coef { get; set; }
        public DbSet<PMT_Household> PMT_Household { get; set; }

        public DbSet<InstallationSetup> InstallationSetup { get; set; }

        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<EnrolmentDetail> EnrolmentDetails { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }

        public DbSet<ClinicVisit> ClinicVisits { get; set; }
        public DbSet<PaymentPoint> PaymentPoints { get; set; }
        public DbSet<BeneficiaryPaymentPoint> BeneficiaryPaymentPoints { get; set; }
        public DbSet<PaymentCycle> PaymentCycles { get; set; }

        public DbSet<PayrollException> PayrollExceptions { get; set; }
        public DbSet<PayrollExceptionType> PayrollExceptionTypes { get; set; }
        public DbSet<PrePayrollAction> PrePayrollActions { get; set; }
        public DbSet<PrePayrollCheck> PrePayrollChecks { get; set; }
        public DbSet<PrePayrollChecksDetail> PrePayrollChecksDetails { get; set; }
        public DbSet<PrePayrollTransaction> PrePayrollTransactions { get; set; }

        public DbSet<FundRequest> FundRequests { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<CaseManagement> CaseManagement { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<PaymentStatus> PaymentStatus { get; set; }

        public DbSet<SystemModule> SystemModules { get; set; }
        public DbSet<MotherPreventiveService> MotherPreventiveServices { get; set; }
        public DbSet<FamilyPlanning> FamilyPlannings { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<DrugsAdministered> DrugsAdministered { get; set; }

        public DbSet<MotherClinicVisit> MotherClinicVisits { get; set; }
        public DbSet<PostNatalExamination> PostNatalExaminations { get; set; }
        public DbSet<PostNatalExaminationDetail> PostNatalExaminationDetails { get; set; }
        public DbSet<PostNatalMilestone> PostNatalMilestones { get; set; }

        public DbSet<PregnancyData> PregnancyData { get; set; }
        public DbSet<Pregnancy> Pregnancies { get; set; }

        public DbSet<ChildHealthRecord> ChildHealthRecords { get; set; }
        public DbSet<ChildFeedingInformation> ChildFeedingInformation { get; set; }
        public DbSet<CivilRegistration> CivilRegistrations { get; set; }

        public DbSet<PMTCTService> PMTCTServices { get; set; }
        public DbSet<PMTCTIntervention> PMTCTInterventions { get; set; }
        public DbSet<CaseManagementStatus> CaseManagementStatus { get; set; }

        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ComplaintStatus> ComplaintStatus { get; set; }
        public DbSet<ComplaintNote> ComplaintNotes { get; set; }
        public DbSet<ComplaintType> ComplaintTypes { get; set; }

        public DbSet<Change> Changes { get; set; }
        public DbSet<ChangeNote> ChangeNotes { get; set; }

        public DbSet<MpesaAuthorizationCode> MpesaAuthorizationCode { get; set; }

        public DbSet<MPesaFeedBack> MPesaFeedBack { get; set; }
        public DbSet<TestData> TestData { get; set; }
        public DbSet<TempTable> TempTable { get; set; }

        public DbSet<FingerPrintVerification> FingerPrintVerifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportCategory> ReportCategories { get; set; }
        public DbSet<Enumerator> Enumerators { get; set; }
        public DbSet<EnumeratorDevice> EnumeratorDevices { get; set; }
        public DbSet<ReportType> ReportTypes { get; set; }

        public DbSet<KakamegaVillage> KakamegaVillages { get; set; }

        public DbSet<KeyMilestone> KeyMilestones { get; set; }

        public DbSet<DevelopmentMilestone> DevelopmentMilestones { get; set; }
        public DbSet<ChildDevelopmentMilestone> ChildDevelopmentMilestones { get; set; }

        public DbSet<CutOff> CutOffs { get; set; }

        public DbSet<SMS> SMS { get; set; }

        public DbSet<ReportingPeriod> ReportingPeriods { get; set; }

        // Dashboards
        public DbSet<RegistrationSummary> RegistrationSummary { get; set; }

        public DbSet<CommunityValidationSummary> CommunityValidationSummary { get; set; }
        public DbSet<EnrolmentSummary> EnrolmentSummary { get; set; }
        public DbSet<PaymentsSummary> PaymentsSummary { get; set; }
        public DbSet<ComplaintsSummary> ComplaintsSummary { get; set; }
        public DbSet<CaseManagementSummary> CaseManagementSummary { get; set; }
    }
}