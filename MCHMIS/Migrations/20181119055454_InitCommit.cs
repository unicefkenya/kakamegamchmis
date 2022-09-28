using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MCHMIS.Migrations
{
    public partial class InitCommit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseManagementStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseManagementStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseManagementSummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PeriodId = table.Column<int>(nullable: false),
                    Updated = table.Column<int>(nullable: false),
                    Missed = table.Column<int>(nullable: false),
                    WardId = table.Column<int>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: true),
                    VisitTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseManagementSummary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChildDevelopmentMilestones",
                columns: table => new
                {
                    ChildId = table.Column<string>(nullable: false),
                    DevelopmentMilestoneId = table.Column<int>(nullable: false),
                    AgeAchieved = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildDevelopmentMilestones", x => new { x.ChildId, x.DevelopmentMilestoneId });
                });

            migrationBuilder.CreateTable(
                name: "CommunityValidationSummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PeriodId = table.Column<int>(nullable: false),
                    High = table.Column<int>(nullable: false),
                    Medium = table.Column<int>(nullable: false),
                    Low = table.Column<int>(nullable: false),
                    WardId = table.Column<int>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityValidationSummary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComplaintsSummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PeriodId = table.Column<int>(nullable: false),
                    OpenWithinSLA = table.Column<int>(nullable: false),
                    OpenOutsideSLA = table.Column<int>(nullable: false),
                    ResolvedWithinSLA = table.Column<int>(nullable: false),
                    ResolvedOutsideSLA = table.Column<int>(nullable: false),
                    WardId = table.Column<int>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintsSummary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComplaintStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DevelopmentMilestones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    NormalLimits = table.Column<string>(nullable: true),
                    UpperLimit = table.Column<int>(nullable: false),
                    LimitTag = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevelopmentMilestones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnrolmentSummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PeriodId = table.Column<int>(nullable: false),
                    Enrolled = table.Column<int>(nullable: false),
                    Active = table.Column<int>(nullable: false),
                    Peak = table.Column<int>(nullable: false),
                    Waiting = table.Column<int>(nullable: false),
                    WardId = table.Column<int>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrolmentSummary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnumeratorDevices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceId = table.Column<string>(nullable: true),
                    DeviceModel = table.Column<string>(nullable: true),
                    DeviceManufacturer = table.Column<string>(nullable: true),
                    DeviceName = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    VersionNumber = table.Column<string>(nullable: true),
                    Platform = table.Column<string>(nullable: true),
                    Idiom = table.Column<string>(nullable: true),
                    IsDevice = table.Column<bool>(nullable: false),
                    EnumeratorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumeratorDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KakamegaVillages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ward = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KakamegaVillages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailSettings",
                columns: table => new
                {
                    SMTPServerAddress = table.Column<string>(nullable: false),
                    SMTPServerPort = table.Column<int>(nullable: false),
                    SMTPServerUserName = table.Column<string>(nullable: true),
                    SMTPServerPassword = table.Column<string>(nullable: true),
                    SenderName = table.Column<string>(nullable: true),
                    SenderEmailAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailSettings", x => x.SMTPServerAddress);
                });

            migrationBuilder.CreateTable(
                name: "MpesaAuthorizationCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true),
                    UtcDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MpesaAuthorizationCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MPesaFeedBack",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentId = table.Column<int>(nullable: false),
                    Result = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MPesaFeedBack", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotesCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotesCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentPoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentsSummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PeriodId = table.Column<int>(nullable: false),
                    IndicatorId = table.Column<int>(nullable: false),
                    Stage1 = table.Column<int>(nullable: false),
                    Stage2 = table.Column<int>(nullable: false),
                    Stage3 = table.Column<int>(nullable: false),
                    Stage4 = table.Column<int>(nullable: false),
                    Stage5 = table.Column<int>(nullable: false),
                    Stage6 = table.Column<int>(nullable: false),
                    Benevolent = table.Column<int>(nullable: false),
                    WardId = table.Column<int>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentsSummary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollExceptionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollExceptionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PMT_Household",
                columns: table => new
                {
                    HouseholdId = table.Column<string>(nullable: false),
                    PMT_Dwelling = table.Column<decimal>(nullable: true),
                    PMT_HHMemberSize = table.Column<decimal>(nullable: true),
                    PMT_HHRoomsPerPersons = table.Column<decimal>(nullable: true),
                    PMT_Wall = table.Column<decimal>(nullable: true),
                    PMT_ROOF = table.Column<decimal>(nullable: true),
                    PMT_FLOOR = table.Column<decimal>(nullable: true),
                    PMT_WATER = table.Column<decimal>(nullable: true),
                    PMT_TOILET = table.Column<decimal>(nullable: true),
                    PMT_COOK = table.Column<decimal>(nullable: true),
                    PMT_LIGHT = table.Column<decimal>(nullable: true),
                    PMT_TV = table.Column<decimal>(nullable: true),
                    PMT_MOTORCYCLE = table.Column<decimal>(nullable: true),
                    PMT_CAR = table.Column<decimal>(nullable: true),
                    PMT_REFRIGERATOR = table.Column<decimal>(nullable: true),
                    PMT_TUKTUK = table.Column<decimal>(nullable: true),
                    PMT_EXOTIC = table.Column<decimal>(nullable: true),
                    PMT_ZEBU = table.Column<decimal>(nullable: true),
                    PMT_SHEEP = table.Column<decimal>(nullable: true),
                    PMT_GOATS = table.Column<decimal>(nullable: true),
                    PMT_CAMELS = table.Column<decimal>(nullable: true),
                    PMT_DONKEYS = table.Column<decimal>(nullable: true),
                    PMT_MALEHEAD = table.Column<decimal>(nullable: true),
                    PMT_SPOUSE = table.Column<decimal>(nullable: true),
                    PMT_MONOGAMOUS = table.Column<decimal>(nullable: true),
                    PMT_MALEMEMBERS = table.Column<decimal>(nullable: true),
                    PMT_HEADAGE = table.Column<decimal>(nullable: true),
                    PMT_MEANHOUSEHOLDAGE = table.Column<decimal>(nullable: true),
                    PMT_SPOUSEAGE = table.Column<decimal>(nullable: true),
                    PMT_DEPENDANCY = table.Column<decimal>(nullable: true),
                    PMT_CHILDRENUNDER6 = table.Column<decimal>(nullable: true),
                    PMT_ORPHAN = table.Column<decimal>(nullable: true),
                    PMT_UNDER12SCHOOL = table.Column<decimal>(nullable: true),
                    PMT_HEADEDUCATION = table.Column<decimal>(nullable: true),
                    PMT_SPOUSEEDUCATION = table.Column<decimal>(nullable: true),
                    PMT_MAXHOUSEHOLDEDUCATION = table.Column<decimal>(nullable: true),
                    PMT_DISABLED15TO65 = table.Column<decimal>(nullable: true),
                    PMT_DEATHINHOUSEHOLD = table.Column<decimal>(nullable: true),
                    PMT_BIRTHINHOUSEHOLD = table.Column<decimal>(nullable: true),
                    PMT_HEADWORK = table.Column<decimal>(nullable: true),
                    PMT_SPOUSEWORK = table.Column<decimal>(nullable: true),
                    PMT_WORKING15TO65 = table.Column<decimal>(nullable: true),
                    PMT_WORKING6TO15 = table.Column<decimal>(nullable: true),
                    PMT_WAGEWORKERSSUBLOCATION = table.Column<decimal>(nullable: true),
                    PMT_AGRIWORKERSSUBLOCATION = table.Column<decimal>(nullable: true),
                    PMT_SELFEMPLOYEDSUBLOCATION = table.Column<decimal>(nullable: true),
                    PMT_POPULATIONSUBLOCATION = table.Column<decimal>(nullable: true),
                    PMT_DEATHRATESUBLOCATION = table.Column<decimal>(nullable: true),
                    PMT_BIRTHRATESUBLOCATION = table.Column<decimal>(nullable: true),
                    PMT_PRECIPITATIONSUBLOCATION = table.Column<decimal>(nullable: true),
                    PMT_ELEVATIONSUBLOCATION = table.Column<decimal>(nullable: true),
                    PMT_SCORE = table.Column<decimal>(nullable: true),
                    Household_Group = table.Column<string>(nullable: true),
                    Household_Rank = table.Column<string>(nullable: true),
                    PWSD = table.Column<bool>(nullable: true),
                    OVC = table.Column<bool>(nullable: true),
                    OP = table.Column<bool>(nullable: true),
                    HSNP = table.Column<bool>(nullable: true),
                    MultipleVulnerability = table.Column<bool>(nullable: true),
                    FinalProgramme = table.Column<string>(nullable: true),
                    LCS = table.Column<decimal>(nullable: true),
                    PMT_Coastal = table.Column<decimal>(nullable: true),
                    PMT_UpperEastern = table.Column<decimal>(nullable: true),
                    PMT_LowerEastern = table.Column<decimal>(nullable: true),
                    PMT_NorthEastern = table.Column<decimal>(nullable: true),
                    PMT_Nyanza = table.Column<decimal>(nullable: true),
                    PMT_NorthRift = table.Column<decimal>(nullable: true),
                    PMT_CentralRift = table.Column<decimal>(nullable: true),
                    PMT_Western = table.Column<decimal>(nullable: true),
                    PWSD_Member = table.Column<string>(nullable: true),
                    OVC_Member = table.Column<string>(nullable: true),
                    OP_Member = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMT_Household", x => x.HouseholdId);
                });

            migrationBuilder.CreateTable(
                name: "PMT_Rural",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    region_Nairobi = table.Column<float>(nullable: true),
                    region_Central = table.Column<float>(nullable: true),
                    region_Mombasa = table.Column<float>(nullable: true),
                    region_Coastal = table.Column<float>(nullable: true),
                    region_Upper_Eastern = table.Column<float>(nullable: true),
                    region_Mid_Eastern = table.Column<float>(nullable: true),
                    region_Lower_Eastern = table.Column<float>(nullable: true),
                    region_North_Eastern = table.Column<float>(nullable: true),
                    region_Nyanza = table.Column<float>(nullable: true),
                    region_North_Rift = table.Column<float>(nullable: true),
                    region_Central_Rift = table.Column<float>(nullable: true),
                    region_South_Rift = table.Column<float>(nullable: true),
                    region_Western = table.Column<float>(nullable: true),
                    cont_population = table.Column<float>(nullable: true),
                    cont_deathsrate = table.Column<float>(nullable: true),
                    cont_birthsrate = table.Column<float>(nullable: true),
                    cont_Mean_precip = table.Column<float>(nullable: true),
                    cont_Mean_elev = table.Column<float>(nullable: true),
                    cont_work_subloc = table.Column<float>(nullable: true),
                    cont_work_agrs = table.Column<float>(nullable: true),
                    cont_selfemp_subloc = table.Column<float>(nullable: true),
                    SubLocVar = table.Column<string>(nullable: true),
                    Subloc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMT_Rural", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PMT_Urban",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    region_Nairobi = table.Column<float>(nullable: true),
                    region_Central = table.Column<float>(nullable: true),
                    region_Mombasa = table.Column<float>(nullable: true),
                    region_Coastal = table.Column<float>(nullable: true),
                    region_Upper_Eastern = table.Column<float>(nullable: true),
                    region_Mid_Eastern = table.Column<float>(nullable: true),
                    region_Lower_Eastern = table.Column<float>(nullable: true),
                    region_North_Eastern = table.Column<float>(nullable: true),
                    region_Nyanza = table.Column<float>(nullable: true),
                    region_North_Rift = table.Column<float>(nullable: true),
                    region_Central_Rift = table.Column<float>(nullable: true),
                    region_South_Rift = table.Column<float>(nullable: true),
                    region_Western = table.Column<float>(nullable: true),
                    cont_population = table.Column<float>(nullable: true),
                    cont_deathsrate = table.Column<float>(nullable: true),
                    cont_birthsrate = table.Column<float>(nullable: true),
                    cont_Mean_precip = table.Column<float>(nullable: true),
                    cont_Mean_elev = table.Column<float>(nullable: true),
                    cont_work_subloc = table.Column<float>(nullable: true),
                    cont_work_agrs = table.Column<float>(nullable: true),
                    cont_selfemp_subloc = table.Column<float>(nullable: true),
                    SubLocVar = table.Column<string>(nullable: true),
                    Subloc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMT_Urban", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PMTCutOffs",
                columns: table => new
                {
                    PMTModelId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    USD = table.Column<float>(nullable: false),
                    KES = table.Column<float>(nullable: false),
                    LogScale = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMTCutOffs", x => x.PMTModelId);
                });

            migrationBuilder.CreateTable(
                name: "PrePayrollActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CanProceed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrePayrollActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programmes",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programmes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reasons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Registration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Over18 = table.Column<bool>(nullable: false),
                    IdentificationFormId = table.Column<int>(nullable: false),
                    WardId = table.Column<int>(nullable: false),
                    Institution = table.Column<string>(nullable: true),
                    Landmark = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    GuardianPhone = table.Column<string>(nullable: true),
                    FingerPrint = table.Column<byte[]>(nullable: true),
                    VerifyingFingerPrint = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationSummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PeriodId = table.Column<int>(nullable: false),
                    Registered = table.Column<int>(nullable: false),
                    Eligible = table.Column<int>(nullable: false),
                    Ineligible = table.Column<int>(nullable: false),
                    WardId = table.Column<int>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationSummary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportingPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportingPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scoring_Coef",
                columns: table => new
                {
                    ItemId = table.Column<string>(nullable: false),
                    Coefficient = table.Column<float>(nullable: true),
                    Mean = table.Column<float>(nullable: true),
                    StdDev = table.Column<float>(nullable: true),
                    Coefficient1 = table.Column<float>(nullable: true),
                    Mean1 = table.Column<float>(nullable: true),
                    StdDev1 = table.Column<float>(nullable: true),
                    Region_Urban = table.Column<string>(nullable: true),
                    Coefficient2 = table.Column<float>(nullable: true),
                    Mean2 = table.Column<float>(nullable: true),
                    StdDev2 = table.Column<float>(nullable: true),
                    Region_Rural = table.Column<string>(nullable: true),
                    F13 = table.Column<string>(nullable: true),
                    F14 = table.Column<string>(nullable: true),
                    F15 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scoring_Coef", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemModules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemModules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemTasks_SystemTasks_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SystemTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TempTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Col1 = table.Column<string>(nullable: true),
                    Col2 = table.Column<string>(nullable: true),
                    Col3 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrePayrollTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BeneficiaryId = table.Column<string>(nullable: true),
                    PrePayrollCheckId = table.Column<int>(nullable: false),
                    PaymentPointId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrePayrollTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrePayrollTransactions_PaymentPoints_PaymentPointId",
                        column: x => x.PaymentPointId,
                        principalTable: "PaymentPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollExceptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ExceptionTypeId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollExceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollExceptions_PayrollExceptionTypes_ExceptionTypeId",
                        column: x => x.ExceptionTypeId,
                        principalTable: "PayrollExceptionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Keyword = table.Column<string>(nullable: true),
                    ReportCategoryId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_ReportCategories_ReportCategoryId",
                        column: x => x.ReportCategoryId,
                        principalTable: "ReportCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SystemModuleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemCodes_SystemModules_SystemModuleId",
                        column: x => x.SystemModuleId,
                        principalTable: "SystemModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleProfiles",
                columns: table => new
                {
                    TaskId = table.Column<int>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleProfiles", x => new { x.TaskId, x.RoleId });
                    table.UniqueConstraint("AK_RoleProfiles_RoleId_TaskId", x => new { x.RoleId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_RoleProfiles_SystemTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "SystemTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemCodeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SystemCodeId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    OrderNo = table.Column<decimal>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemCodeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemCodeDetails_SystemCodes_SystemCodeId",
                        column: x => x.SystemCodeId,
                        principalTable: "SystemCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClinicVisits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    VisitTypeId = table.Column<int>(nullable: true),
                    PaymentPointId = table.Column<int>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicVisits_PaymentPoints_PaymentPointId",
                        column: x => x.PaymentPointId,
                        principalTable: "PaymentPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClinicVisits_SystemCodeDetails_VisitTypeId",
                        column: x => x.VisitTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComplaintTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false),
                    SLAResolutionTime = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplaintTypes_SystemCodeDetails_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DrugsAdministered",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeliveryId = table.Column<string>(nullable: true),
                    DrugId = table.Column<int>(nullable: false),
                    RecipientTypeId = table.Column<int>(nullable: false),
                    RecipientId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugsAdministered", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrugsAdministered_SystemCodeDetails_DrugId",
                        column: x => x.DrugId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdRegCharacteristics",
                columns: table => new
                {
                    HouseholdId = table.Column<string>(nullable: false),
                    HabitableRoomsNo = table.Column<int>(nullable: true),
                    IsOwnHouse = table.Column<bool>(nullable: false),
                    TenureStatusId = table.Column<int>(nullable: false),
                    TenureOwnerOccupiedId = table.Column<int>(nullable: false),
                    TenureStatusOther = table.Column<string>(nullable: true),
                    RoofMaterialId = table.Column<int>(nullable: false),
                    RoofMaterialOther = table.Column<string>(nullable: true),
                    WallMaterialId = table.Column<int>(nullable: false),
                    WallMaterialOther = table.Column<string>(nullable: true),
                    FloorMaterialId = table.Column<int>(nullable: false),
                    FloorMaterialOther = table.Column<string>(nullable: true),
                    UnitRiskId = table.Column<int>(nullable: false),
                    UnitRiskOther = table.Column<string>(nullable: true),
                    WaterSourceId = table.Column<int>(nullable: false),
                    WaterSourceOther = table.Column<string>(nullable: true),
                    ToiletTypeId = table.Column<int>(nullable: false),
                    ToiletTypeOther = table.Column<string>(nullable: true),
                    CookingFuelSourceId = table.Column<int>(nullable: false),
                    CookingFuelSourceOther = table.Column<string>(nullable: true),
                    LightingSourceId = table.Column<int>(nullable: false),
                    LightingSourceOther = table.Column<string>(nullable: true),
                    LiveBirths = table.Column<int>(nullable: false),
                    Deaths = table.Column<int>(nullable: false),
                    HouseholdConditionId = table.Column<int>(nullable: false),
                    HasSkippedMealId = table.Column<int>(nullable: false),
                    IsRecievingNSNPBenefit = table.Column<bool>(nullable: false),
                    IsReceivingOtherBenefitId = table.Column<int>(nullable: true),
                    HasBeenInMCHProgramId = table.Column<int>(nullable: false),
                    OtherProgrammes = table.Column<string>(nullable: true),
                    OtherProgrammesBenefitTypeId = table.Column<int>(nullable: true),
                    OtherProgrammesBenefitAmount = table.Column<decimal>(nullable: true),
                    OtherProgrammesInKindBenefit = table.Column<string>(nullable: true),
                    BenefitFromFriendsRelativeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdRegCharacteristics", x => x.HouseholdId);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_BenefitFromFriendsRelativeId",
                        column: x => x.BenefitFromFriendsRelativeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_CookingFuelSourceId",
                        column: x => x.CookingFuelSourceId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_FloorMaterialId",
                        column: x => x.FloorMaterialId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_HasSkippedMealId",
                        column: x => x.HasSkippedMealId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_HouseholdConditionId",
                        column: x => x.HouseholdConditionId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_IsReceivingOtherBenefitId",
                        column: x => x.IsReceivingOtherBenefitId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_LightingSourceId",
                        column: x => x.LightingSourceId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_OtherProgrammesBenefitTypeId",
                        column: x => x.OtherProgrammesBenefitTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_RoofMaterialId",
                        column: x => x.RoofMaterialId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_TenureOwnerOccupiedId",
                        column: x => x.TenureOwnerOccupiedId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_TenureStatusId",
                        column: x => x.TenureStatusId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_ToiletTypeId",
                        column: x => x.ToiletTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_UnitRiskId",
                        column: x => x.UnitRiskId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_WallMaterialId",
                        column: x => x.WallMaterialId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegCharacteristics_SystemCodeDetails_WaterSourceId",
                        column: x => x.WaterSourceId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PMTWeights",
                columns: table => new
                {
                    ConstantId = table.Column<int>(nullable: false),
                    LocalityId = table.Column<int>(nullable: false),
                    Weight = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMTWeights", x => new { x.ConstantId, x.LocalityId });
                    table.ForeignKey(
                        name: "FK_PMTWeights_SystemCodeDetails_ConstantId",
                        column: x => x.ConstantId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PMTWeights_SystemCodeDetails_LocalityId",
                        column: x => x.LocalityId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KeyMilestones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    ClinicVisitId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyMilestones_ClinicVisits_ClinicVisitId",
                        column: x => x.ClinicVisitId,
                        principalTable: "ClinicVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClinicVisitId = table.Column<int>(nullable: true),
                    TriggerEvent = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SMS_ClinicVisits_ClinicVisitId",
                        column: x => x.ClinicVisitId,
                        principalTable: "ClinicVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Changes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    HouseholdId = table.Column<string>(nullable: true),
                    ChangeTypeId = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    ApprovalNotes = table.Column<string>(nullable: true),
                    NotificationDate = table.Column<DateTime>(nullable: true),
                    DeathDate = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    DateActioned = table.Column<DateTime>(nullable: true),
                    ActionedById = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    SupportingDocument = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    OwnsPhone = table.Column<bool>(nullable: false),
                    OwnsPhoneId = table.Column<int>(nullable: false),
                    NomineeFirstName = table.Column<string>(nullable: true),
                    NomineeMiddleName = table.Column<string>(nullable: true),
                    NomineeSurname = table.Column<string>(nullable: true),
                    NomineeIdNumber = table.Column<string>(nullable: true),
                    NOKFirstName = table.Column<string>(nullable: true),
                    NOKMiddleName = table.Column<string>(nullable: true),
                    NOKSurname = table.Column<string>(nullable: true),
                    NOKPhone = table.Column<string>(maxLength: 10, nullable: true),
                    NOKIdNumber = table.Column<string>(nullable: true),
                    ChildId = table.Column<string>(nullable: true),
                    FingerPrintVerified = table.Column<bool>(nullable: false),
                    TakingFingerPrint = table.Column<bool>(nullable: false),
                    FingerPrint = table.Column<byte[]>(nullable: true),
                    RawFingerPrint = table.Column<byte[]>(nullable: true),
                    RequiresIPRSCheck = table.Column<bool>(nullable: false),
                    IPRSVerified = table.Column<bool>(nullable: true),
                    IPRSPassed = table.Column<bool>(nullable: true),
                    IPRSExceptionId = table.Column<int>(nullable: true),
                    RequiresMPESACheck = table.Column<bool>(nullable: false),
                    MPESACheckStatusId = table.Column<int>(nullable: true),
                    DateMPESAVerified = table.Column<DateTime>(nullable: true),
                    CustomerType = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    RecipientNames = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Changes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Changes_SystemCodeDetails_ChangeTypeId",
                        column: x => x.ChangeTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Changes_Status_MPESACheckStatusId",
                        column: x => x.MPESACheckStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Changes_ApprovalStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CVListDetails",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ListId = table.Column<string>(nullable: true),
                    HouseholdId = table.Column<string>(nullable: true),
                    CVHouseHoldId = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: true),
                    Variance = table.Column<float>(nullable: true),
                    VarianceCategoryId = table.Column<int>(nullable: true),
                    ApprovalStatusId = table.Column<int>(nullable: true),
                    ActionedById = table.Column<string>(nullable: true),
                    DateActioned = table.Column<DateTime>(nullable: true),
                    DateSubmitedByCHV = table.Column<DateTime>(nullable: true),
                    EnumeratorId = table.Column<int>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    InterviewStatusId = table.Column<int>(nullable: true),
                    InterviewResultId = table.Column<int>(nullable: true),
                    CannotFindHouseholdReasonId = table.Column<int>(nullable: true),
                    ValidationNotes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVListDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CVListDetails_ApprovalStatus_ApprovalStatusId",
                        column: x => x.ApprovalStatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CVListDetails_SystemCodeDetails_CannotFindHouseholdReasonId",
                        column: x => x.CannotFindHouseholdReasonId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CVListDetails_SystemCodeDetails_InterviewResultId",
                        column: x => x.InterviewResultId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CVListDetails_SystemCodeDetails_InterviewStatusId",
                        column: x => x.InterviewStatusId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CVListDetails_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CVListDetails_SystemCodeDetails_VarianceCategoryId",
                        column: x => x.VarianceCategoryId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CVLists",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    DateApproved = table.Column<DateTime>(nullable: true),
                    ApprovedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    Households = table.Column<int>(nullable: false),
                    Captured = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    ApprovalNotes = table.Column<string>(nullable: true),
                    CaptureNotes = table.Column<string>(nullable: true),
                    ListTypeId = table.Column<int>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CVLists_SystemCodeDetails_ListTypeId",
                        column: x => x.ListTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CVLists_ApprovalStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrolments",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    DateApproved = table.Column<DateTime>(nullable: true),
                    ApprovedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HouseholdsGenerated = table.Column<int>(nullable: false),
                    HouseholdsValidated = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    ApprovalNotes = table.Column<string>(nullable: true),
                    DateImported = table.Column<DateTime>(nullable: true),
                    ImportedById = table.Column<string>(nullable: true),
                    ImportStatusId = table.Column<int>(nullable: true),
                    ImportApprovedById = table.Column<string>(nullable: true),
                    ImportDateApproved = table.Column<DateTime>(nullable: true),
                    ImportApprovalNotes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrolments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrolments_ApprovalStatus_ImportStatusId",
                        column: x => x.ImportStatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrolments_ApprovalStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundRequests",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    DateApproved = table.Column<DateTime>(nullable: true),
                    ApprovedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CycleId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Beneficiaries = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundRequests_ApprovalStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payrolls",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    DateApproved = table.Column<DateTime>(nullable: true),
                    ApprovedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FundRequestId = table.Column<int>(nullable: false),
                    CycleId = table.Column<int>(nullable: false),
                    Reconciled = table.Column<bool>(nullable: false),
                    DateReconciled = table.Column<DateTime>(nullable: true),
                    ReconciledById = table.Column<string>(nullable: true),
                    ReconciliationDateApproved = table.Column<DateTime>(nullable: true),
                    ReconciliationApprovedById = table.Column<string>(nullable: true),
                    ReconciliationStatusId = table.Column<int>(nullable: true),
                    ReconciliationNotes = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Beneficiaries = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payrolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payrolls_FundRequests_FundRequestId",
                        column: x => x.FundRequestId,
                        principalTable: "FundRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payrolls_ApprovalStatus_ReconciliationStatusId",
                        column: x => x.ReconciliationStatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payrolls_ApprovalStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrePayrollChecks",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    DateApproved = table.Column<DateTime>(nullable: true),
                    ApprovedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentCycleId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    ExpectedAmount = table.Column<decimal>(nullable: false),
                    DuplicateIds = table.Column<int>(nullable: true),
                    DuplicatePhoneNumbers = table.Column<int>(nullable: true),
                    UnusualAmounts = table.Column<int>(nullable: true),
                    RecipientChange = table.Column<int>(nullable: true),
                    PhoneNumberIssues = table.Column<int>(nullable: true),
                    Imported = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrePayrollChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrePayrollChecks_ApprovalStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrePayrollChecksDetails",
                columns: table => new
                {
                    DateApproved = table.Column<DateTime>(nullable: true),
                    ApprovedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PrePayrollCheckId = table.Column<int>(nullable: false),
                    BeneficiaryId = table.Column<string>(nullable: true),
                    ExceptionId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: true),
                    RecipientName = table.Column<string>(nullable: true),
                    RecipientPhone = table.Column<string>(nullable: true),
                    ActionId = table.Column<int>(nullable: true),
                    PaymentId = table.Column<int>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    CustomerType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrePayrollChecksDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrePayrollChecksDetails_PrePayrollActions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "PrePayrollActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrePayrollChecksDetails_PayrollExceptions_ExceptionId",
                        column: x => x.ExceptionId,
                        principalTable: "PayrollExceptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrePayrollChecksDetails_PrePayrollChecks_PrePayrollCheckId",
                        column: x => x.PrePayrollCheckId,
                        principalTable: "PrePayrollChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrePayrollChecksDetails_ApprovalStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "AuditTrail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    RequestIpAddress = table.Column<string>(nullable: true),
                    ChangeType = table.Column<string>(nullable: true),
                    TableName = table.Column<string>(nullable: true),
                    RecordId = table.Column<string>(nullable: true),
                    OldValue = table.Column<string>(nullable: true),
                    NewValue = table.Column<string>(nullable: true),
                    PCName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BeneficiaryPaymentPoints",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentPointId = table.Column<int>(nullable: false),
                    HouseholdId = table.Column<string>(nullable: true),
                    CaseManagementId = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficiaryPaymentPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeneficiaryPaymentPoints_PaymentPoints_PaymentPointId",
                        column: x => x.PaymentPointId,
                        principalTable: "PaymentPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BeneficiaryPaymentPoints_PaymentStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "PaymentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseManagement",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    HouseholdId = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: true),
                    ReasonId = table.Column<int>(nullable: true),
                    DateExited = table.Column<DateTime>(nullable: true),
                    LastVisit = table.Column<DateTime>(nullable: true),
                    NextVisit = table.Column<DateTime>(nullable: true),
                    NextVisitClinicId = table.Column<int>(nullable: true),
                    MissedVisits = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseManagement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseManagement_Reasons_ReasonId",
                        column: x => x.ReasonId,
                        principalTable: "Reasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseManagement_CaseManagementStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "CaseManagementStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeNotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Note = table.Column<string>(nullable: true),
                    ChangeId = table.Column<string>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangeNotes_Changes_ChangeId",
                        column: x => x.ChangeId,
                        principalTable: "Changes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChildHealthRecords",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    ChildId = table.Column<string>(nullable: false),
                    CaseManagementId = table.Column<string>(nullable: true),
                    PregnancyId = table.Column<string>(nullable: true),
                    DeliveryPlaceId = table.Column<int>(nullable: false),
                    OtherDeliveryPlace = table.Column<string>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: true),
                    HealthFacilityName = table.Column<string>(nullable: true),
                    BirthNotificationNumber = table.Column<string>(nullable: true),
                    PermanentRegisterNumber = table.Column<string>(nullable: true),
                    CWCNumber = table.Column<string>(nullable: true),
                    MFLNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildHealthRecords", x => x.ChildId);
                    table.ForeignKey(
                        name: "FK_ChildHealthRecords_SystemCodeDetails_DeliveryPlaceId",
                        column: x => x.DeliveryPlaceId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComplaintNotes",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    ComplaintId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplaintNotes_SystemCodeDetails_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    DateApproved = table.Column<DateTime>(nullable: true),
                    ApprovedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    IsComplainantAnonymousId = table.Column<int>(nullable: false),
                    IsComplainantBeneficiaryId = table.Column<int>(nullable: true),
                    UniqueId = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    ComplaintTypeId = table.Column<int>(nullable: true),
                    OtherComplaintType = table.Column<string>(nullable: true),
                    ComplaintDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(maxLength: 10, nullable: true),
                    NOKName = table.Column<string>(nullable: true),
                    NOKPhone = table.Column<string>(maxLength: 10, nullable: true),
                    NOKIdNumber = table.Column<string>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: false),
                    VillageId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ResolvedById = table.Column<string>(nullable: true),
                    DateResolved = table.Column<DateTime>(nullable: true),
                    ClosedById = table.Column<string>(nullable: true),
                    DateClosed = table.Column<DateTime>(nullable: true),
                    EscalationDate = table.Column<DateTime>(nullable: true),
                    EscalatedById = table.Column<string>(nullable: true),
                    ActionTaken = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    Form = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Complaints_SystemCodeDetails_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaints_ComplaintTypes_ComplaintTypeId",
                        column: x => x.ComplaintTypeId,
                        principalTable: "ComplaintTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaints_SystemCodeDetails_IsComplainantAnonymousId",
                        column: x => x.IsComplainantAnonymousId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaints_SystemCodeDetails_IsComplainantBeneficiaryId",
                        column: x => x.IsComplainantBeneficiaryId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaints_ComplaintStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ComplaintStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 20, nullable: false),
                    GeoMasterId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Constituencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CountyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constituencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Constituencies_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubCounties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CountyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCounties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCounties_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HealthFacilities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SubCountyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthFacilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthFacilities_SubCounties_SubCountyId",
                        column: x => x.SubCountyId,
                        principalTable: "SubCounties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SubCountyId = table.Column<int>(nullable: true),
                    ConstituencyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wards_Constituencies_ConstituencyId",
                        column: x => x.ConstituencyId,
                        principalTable: "Constituencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wards_SubCounties_SubCountyId",
                        column: x => x.SubCountyId,
                        principalTable: "SubCounties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    LastPasswordChangedDate = table.Column<DateTime>(nullable: true),
                    LastActivityDate = table.Column<DateTime>(nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    MacAddress = table.Column<string>(nullable: true),
                    IsLoggedIn = table.Column<bool>(nullable: false),
                    SubCountyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_HealthFacilities_HealthFacilityId",
                        column: x => x.HealthFacilityId,
                        principalTable: "HealthFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_SubCounties_SubCountyId",
                        column: x => x.SubCountyId,
                        principalTable: "SubCounties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstallationSetup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HealthFacilityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallationSetup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstallationSetup_HealthFacilities_HealthFacilityId",
                        column: x => x.HealthFacilityId,
                        principalTable: "HealthFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FingerPrintVerifications",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    HouseholdId = table.Column<string>(nullable: false),
                    VisitDate = table.Column<DateTime>(nullable: false),
                    HealthFacilityId = table.Column<int>(nullable: false),
                    Verified = table.Column<bool>(nullable: false),
                    IsVerifying = table.Column<bool>(nullable: false),
                    TypeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FingerPrintVerifications", x => new { x.Id, x.VisitDate });
                    table.UniqueConstraint("AK_FingerPrintVerifications_HouseholdId_Id", x => new { x.HouseholdId, x.Id });
                    table.ForeignKey(
                        name: "FK_FingerPrintVerifications_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeoMaster",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsDefault = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeoMaster_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeoMaster_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentCycles",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Closed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentCycles_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pregnancies",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    CaseManagementId = table.Column<string>(nullable: true),
                    BloodGroupId = table.Column<int>(nullable: true),
                    RhesusId = table.Column<int>(nullable: true),
                    SupportStatusId = table.Column<int>(nullable: true),
                    PregnancyNo = table.Column<int>(nullable: true),
                    InfantFeedingCounselingDoneId = table.Column<int>(nullable: true),
                    BreastfeedingCounselingDoneId = table.Column<int>(nullable: true),
                    StatusId = table.Column<int>(nullable: true),
                    ReasonId = table.Column<int>(nullable: true),
                    DateExited = table.Column<DateTime>(nullable: true),
                    LastVisit = table.Column<DateTime>(nullable: true),
                    NextVisit = table.Column<DateTime>(nullable: true),
                    NextVisitClinicId = table.Column<int>(nullable: true),
                    MissedVisits = table.Column<int>(nullable: false),
                    LMP = table.Column<DateTime>(nullable: true),
                    LMPUnknown = table.Column<bool>(nullable: true),
                    EPM = table.Column<int>(nullable: true),
                    EDD = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pregnancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pregnancies_SystemCodeDetails_BloodGroupId",
                        column: x => x.BloodGroupId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pregnancies_SystemCodeDetails_BreastfeedingCounselingDoneId",
                        column: x => x.BreastfeedingCounselingDoneId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pregnancies_CaseManagement_CaseManagementId",
                        column: x => x.CaseManagementId,
                        principalTable: "CaseManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pregnancies_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pregnancies_SystemCodeDetails_InfantFeedingCounselingDoneId",
                        column: x => x.InfantFeedingCounselingDoneId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pregnancies_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pregnancies_Reasons_ReasonId",
                        column: x => x.ReasonId,
                        principalTable: "Reasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pregnancies_SystemCodeDetails_RhesusId",
                        column: x => x.RhesusId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pregnancies_CaseManagementStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "CaseManagementStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pregnancies_SystemCodeDetails_SupportStatusId",
                        column: x => x.SupportStatusId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    GeoMasterId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    ConstituencyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Constituencies_ConstituencyId",
                        column: x => x.ConstituencyId,
                        principalTable: "Constituencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Districts_GeoMaster_GeoMasterId",
                        column: x => x.GeoMasterId,
                        principalTable: "GeoMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChildFeedingInformation",
                columns: table => new
                {
                    ChildId = table.Column<string>(nullable: false),
                    CaseManagementId = table.Column<string>(nullable: true),
                    PregnancyId = table.Column<string>(nullable: true),
                    BreastfeedingId = table.Column<string>(nullable: true),
                    BreastfeedingId1 = table.Column<int>(nullable: true),
                    OtherFoodIntroducedId = table.Column<string>(nullable: true),
                    OtherFoodIntroducedId1 = table.Column<int>(nullable: true),
                    OtherFoodIntroducedAge = table.Column<string>(nullable: true),
                    ComplimentaryFoodIntroducedId = table.Column<string>(nullable: true),
                    ComplimentaryFoodIntroducedId1 = table.Column<int>(nullable: true),
                    FeedRetention = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildFeedingInformation", x => x.ChildId);
                    table.ForeignKey(
                        name: "FK_ChildFeedingInformation_SystemCodeDetails_BreastfeedingId1",
                        column: x => x.BreastfeedingId1,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChildFeedingInformation_SystemCodeDetails_ComplimentaryFoodIntroducedId1",
                        column: x => x.ComplimentaryFoodIntroducedId1,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChildFeedingInformation_SystemCodeDetails_OtherFoodIntroducedId1",
                        column: x => x.OtherFoodIntroducedId1,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChildFeedingInformation_Pregnancies_PregnancyId",
                        column: x => x.PregnancyId,
                        principalTable: "Pregnancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CivilRegistrations",
                columns: table => new
                {
                    ChildId = table.Column<string>(nullable: false),
                    CaseManagementId = table.Column<string>(nullable: true),
                    PregnancyId = table.Column<string>(nullable: true),
                    BirthCertificateAvailable = table.Column<bool>(nullable: false),
                    BirthCertificateNumber = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: true),
                    RegistrationPlace = table.Column<string>(nullable: true),
                    BirthCertificate = table.Column<string>(nullable: true),
                    SpecialBirth = table.Column<string>(nullable: true),
                    BirthMark = table.Column<string>(nullable: true),
                    CongenitalAbnormalities = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CivilRegistrations", x => x.ChildId);
                    table.ForeignKey(
                        name: "FK_CivilRegistrations_Pregnancies_PregnancyId",
                        column: x => x.PregnancyId,
                        principalTable: "Pregnancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FamilyPlannings",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    CaseManagementId = table.Column<string>(nullable: true),
                    PregnancyId = table.Column<string>(nullable: true),
                    VisitDate = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    NextVisit = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyPlannings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FamilyPlannings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FamilyPlannings_Pregnancies_PregnancyId",
                        column: x => x.PregnancyId,
                        principalTable: "Pregnancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MotherClinicVisits",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    HouseholdId = table.Column<string>(nullable: true),
                    CaseManagementId = table.Column<string>(nullable: true),
                    PregnancyId = table.Column<string>(nullable: true),
                    ClinicVisitId = table.Column<int>(nullable: true),
                    TypeId = table.Column<int>(nullable: false),
                    HealthFacilityId = table.Column<int>(nullable: true),
                    VisitDate = table.Column<DateTime>(nullable: true),
                    DueDate = table.Column<DateTime>(nullable: true),
                    RequiresFingerPrint = table.Column<bool>(nullable: false),
                    Verified = table.Column<bool>(nullable: false),
                    IsVerifying = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotherClinicVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotherClinicVisits_CaseManagement_CaseManagementId",
                        column: x => x.CaseManagementId,
                        principalTable: "CaseManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MotherClinicVisits_ClinicVisits_ClinicVisitId",
                        column: x => x.ClinicVisitId,
                        principalTable: "ClinicVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MotherClinicVisits_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MotherClinicVisits_Pregnancies_PregnancyId",
                        column: x => x.PregnancyId,
                        principalTable: "Pregnancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PMTCTServices",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    CaseManagementId = table.Column<string>(nullable: true),
                    PregnancyId = table.Column<string>(nullable: true),
                    RecipientTypeId = table.Column<int>(nullable: false),
                    DateGiven = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMTCTServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PMTCTServices_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PMTCTServices_Pregnancies_PregnancyId",
                        column: x => x.PregnancyId,
                        principalTable: "Pregnancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    DistrictId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Divisions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Divisions_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Divisions_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    PregnancyId = table.Column<string>(nullable: true),
                    PregnancyOutcomeId = table.Column<int>(nullable: true),
                    PregnancyDuration = table.Column<int>(nullable: true),
                    HIVTestedId = table.Column<int>(nullable: true),
                    SupportStatusId = table.Column<int>(nullable: true),
                    DeliveryModeId = table.Column<int>(nullable: true),
                    DeliveryDate = table.Column<DateTime>(nullable: false),
                    BloodLossId = table.Column<int>(nullable: true),
                    ObstructedLabourId = table.Column<int>(nullable: true),
                    MotherCondition = table.Column<string>(nullable: true),
                    RescuscitationDoneId = table.Column<int>(nullable: true),
                    MeconiumStainedLiquorId = table.Column<int>(nullable: true),
                    MotherClinicVisitId = table.Column<string>(nullable: true),
                    NextVisit = table.Column<DateTime>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deliveries_SystemCodeDetails_BloodLossId",
                        column: x => x.BloodLossId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_SystemCodeDetails_DeliveryModeId",
                        column: x => x.DeliveryModeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_SystemCodeDetails_HIVTestedId",
                        column: x => x.HIVTestedId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_SystemCodeDetails_MeconiumStainedLiquorId",
                        column: x => x.MeconiumStainedLiquorId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_MotherClinicVisits_MotherClinicVisitId",
                        column: x => x.MotherClinicVisitId,
                        principalTable: "MotherClinicVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_SystemCodeDetails_ObstructedLabourId",
                        column: x => x.ObstructedLabourId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_Pregnancies_PregnancyId",
                        column: x => x.PregnancyId,
                        principalTable: "Pregnancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_SystemCodeDetails_PregnancyOutcomeId",
                        column: x => x.PregnancyOutcomeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_SystemCodeDetails_RescuscitationDoneId",
                        column: x => x.RescuscitationDoneId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_SystemCodeDetails_SupportStatusId",
                        column: x => x.SupportStatusId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostNatalExaminations",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    CaseManagementId = table.Column<string>(nullable: true),
                    PregnancyId = table.Column<string>(nullable: true),
                    VisitDate = table.Column<DateTime>(nullable: false),
                    NextDate = table.Column<DateTime>(nullable: true),
                    ClinicVisitId = table.Column<int>(nullable: false),
                    BloodPressure = table.Column<string>(nullable: true),
                    GeneralCondition = table.Column<string>(nullable: true),
                    FPCounselingId = table.Column<int>(nullable: true),
                    SupportStatusId = table.Column<int>(nullable: true),
                    FPMethodId = table.Column<int>(nullable: true),
                    MotherClinicVisitId = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostNatalExaminations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminations_ClinicVisits_ClinicVisitId",
                        column: x => x.ClinicVisitId,
                        principalTable: "ClinicVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminations_SystemCodeDetails_FPCounselingId",
                        column: x => x.FPCounselingId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminations_SystemCodeDetails_FPMethodId",
                        column: x => x.FPMethodId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminations_MotherClinicVisits_MotherClinicVisitId",
                        column: x => x.MotherClinicVisitId,
                        principalTable: "MotherClinicVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminations_Pregnancies_PregnancyId",
                        column: x => x.PregnancyId,
                        principalTable: "Pregnancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminations_SystemCodeDetails_SupportStatusId",
                        column: x => x.SupportStatusId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PregnancyData",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    PregnancyId = table.Column<string>(nullable: true),
                    ClinicVisitId = table.Column<int>(nullable: true),
                    HouseholdId = table.Column<string>(nullable: true),
                    VisitDate = table.Column<DateTime>(nullable: true),
                    MotherClinicVisitId = table.Column<string>(nullable: true),
                    Weight = table.Column<decimal>(nullable: true),
                    BloodPressure = table.Column<string>(nullable: true),
                    NextVisit = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PregnancyData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PregnancyData_ClinicVisits_ClinicVisitId",
                        column: x => x.ClinicVisitId,
                        principalTable: "ClinicVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PregnancyData_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PregnancyData_MotherClinicVisits_MotherClinicVisitId",
                        column: x => x.MotherClinicVisitId,
                        principalTable: "MotherClinicVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PregnancyData_Pregnancies_PregnancyId",
                        column: x => x.PregnancyId,
                        principalTable: "Pregnancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PMTCTInterventions",
                columns: table => new
                {
                    PMTCTServiceId = table.Column<string>(nullable: false),
                    InterventionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMTCTInterventions", x => new { x.PMTCTServiceId, x.InterventionId });
                    table.UniqueConstraint("AK_PMTCTInterventions_InterventionId_PMTCTServiceId", x => new { x.InterventionId, x.PMTCTServiceId });
                    table.ForeignKey(
                        name: "FK_PMTCTInterventions_SystemCodeDetails_InterventionId",
                        column: x => x.InterventionId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PMTCTInterventions_PMTCTServices_PMTCTServiceId",
                        column: x => x.PMTCTServiceId,
                        principalTable: "PMTCTServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    DivisionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locations_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locations_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Children",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    GenderId = table.Column<int>(nullable: true),
                    DOB = table.Column<DateTime>(nullable: true),
                    DateFirstSeen = table.Column<string>(nullable: true),
                    DeliveryId = table.Column<string>(nullable: true),
                    GestationAtBirth = table.Column<int>(nullable: true),
                    BirthWeight = table.Column<string>(nullable: true),
                    BirthLength = table.Column<string>(nullable: true),
                    HeadCircumference = table.Column<string>(nullable: true),
                    OtherBirthCharacteristics = table.Column<string>(nullable: true),
                    BirthOrder = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: true),
                    DeliveryPlaceId = table.Column<int>(nullable: true),
                    DeliveryHealthFacilityId = table.Column<int>(nullable: true),
                    OtherDeliveryHealthFacility = table.Column<string>(nullable: true),
                    OtherDeliveryPlace = table.Column<string>(nullable: true),
                    DeliveryAssistantId = table.Column<int>(nullable: true),
                    OtherDeliveryAssistant = table.Column<string>(nullable: true),
                    ImmunizedId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Children", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Children_SystemCodeDetails_DeliveryAssistantId",
                        column: x => x.DeliveryAssistantId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Children_HealthFacilities_DeliveryHealthFacilityId",
                        column: x => x.DeliveryHealthFacilityId,
                        principalTable: "HealthFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Children_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Children_SystemCodeDetails_DeliveryPlaceId",
                        column: x => x.DeliveryPlaceId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Children_SystemCodeDetails_GenderId",
                        column: x => x.GenderId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Children_SystemCodeDetails_ImmunizedId",
                        column: x => x.ImmunizedId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Children_SystemCodeDetails_StatusId",
                        column: x => x.StatusId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MotherPreventiveServices",
                columns: table => new
                {
                    PregnancyDataId = table.Column<string>(nullable: false),
                    PreventiveServiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotherPreventiveServices", x => new { x.PregnancyDataId, x.PreventiveServiceId });
                    table.ForeignKey(
                        name: "FK_MotherPreventiveServices_PregnancyData_PregnancyDataId",
                        column: x => x.PregnancyDataId,
                        principalTable: "PregnancyData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MotherPreventiveServices_SystemCodeDetails_PreventiveServiceId",
                        column: x => x.PreventiveServiceId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubLocations",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    RuralUrban = table.Column<string>(nullable: true),
                    WardId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubLocations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubLocations_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostNatalExaminationDetails",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ChildId = table.Column<string>(nullable: true),
                    PostNatalExaminationId = table.Column<string>(nullable: true),
                    BabyGeneralCondition = table.Column<string>(nullable: true),
                    FeedingMethodId = table.Column<int>(nullable: false),
                    ImmunizationStartedId = table.Column<int>(nullable: true),
                    ARTProphylaxisGivenId = table.Column<int>(nullable: true),
                    CPInitiatedId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostNatalExaminationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminationDetails_SystemCodeDetails_ARTProphylaxisGivenId",
                        column: x => x.ARTProphylaxisGivenId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminationDetails_SystemCodeDetails_CPInitiatedId",
                        column: x => x.CPInitiatedId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminationDetails_Children_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Children",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminationDetails_SystemCodeDetails_FeedingMethodId",
                        column: x => x.FeedingMethodId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminationDetails_SystemCodeDetails_ImmunizationStartedId",
                        column: x => x.ImmunizationStartedId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalExaminationDetails_PostNatalExaminations_PostNatalExaminationId",
                        column: x => x.PostNatalExaminationId,
                        principalTable: "PostNatalExaminations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CutOffs",
                columns: table => new
                {
                    SubLocationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubLocationId1 = table.Column<int>(nullable: true),
                    Value = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CutOffs", x => x.SubLocationId);
                    table.ForeignKey(
                        name: "FK_CutOffs_SubLocations_SubLocationId1",
                        column: x => x.SubLocationId1,
                        principalTable: "SubLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Villages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Code = table.Column<string>(nullable: true),
                    SubLocationId = table.Column<int>(nullable: true),
                    WardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Villages_SubLocations_SubLocationId",
                        column: x => x.SubLocationId,
                        principalTable: "SubLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Villages_Wards_WardId",
                        column: x => x.WardId,
                        principalTable: "Wards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostNatalMilestones",
                columns: table => new
                {
                    PostNatalExaminationDetailId = table.Column<string>(nullable: false),
                    KeyMilestoneId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostNatalMilestones", x => new { x.PostNatalExaminationDetailId, x.KeyMilestoneId });
                    table.UniqueConstraint("AK_PostNatalMilestones_KeyMilestoneId_PostNatalExaminationDetailId", x => new { x.KeyMilestoneId, x.PostNatalExaminationDetailId });
                    table.ForeignKey(
                        name: "FK_PostNatalMilestones_KeyMilestones_KeyMilestoneId",
                        column: x => x.KeyMilestoneId,
                        principalTable: "KeyMilestones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNatalMilestones_PostNatalExaminationDetails_PostNatalExaminationDetailId",
                        column: x => x.PostNatalExaminationDetailId,
                        principalTable: "PostNatalExaminationDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommunityAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VillageId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityAreas_Villages_VillageId",
                        column: x => x.VillageId,
                        principalTable: "Villages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enumerators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EnumeratorGroupId = table.Column<int>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    NationalIdNo = table.Column<string>(nullable: true),
                    MobileNo = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LoginDate = table.Column<DateTime>(nullable: true),
                    ActivityDate = table.Column<DateTime>(nullable: true),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false),
                    DeactivatedBy = table.Column<int>(nullable: true),
                    DeactivatedOn = table.Column<DateTime>(nullable: true),
                    VillageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enumerators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enumerators_SystemCodeDetails_EnumeratorGroupId",
                        column: x => x.EnumeratorGroupId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enumerators_Villages_VillageId",
                        column: x => x.VillageId,
                        principalTable: "Villages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdRegs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UniqueId = table.Column<string>(nullable: true),
                    OldUniqueId = table.Column<string>(nullable: true),
                    SubLocationId = table.Column<int>(nullable: true),
                    VillageId = table.Column<int>(nullable: true),
                    CommunityAreaId = table.Column<int>(nullable: true),
                    WardId = table.Column<int>(nullable: true),
                    TempWard = table.Column<string>(nullable: true),
                    CountyId = table.Column<int>(nullable: true),
                    OtherCountyId = table.Column<int>(nullable: true),
                    PhysicalAddress = table.Column<string>(nullable: true),
                    NearestReligiousBuilding = table.Column<string>(nullable: true),
                    NearestSchool = table.Column<string>(nullable: true),
                    ResidenceDurationYears = table.Column<int>(nullable: true),
                    ResidenceDurationMonths = table.Column<int>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    ReasonId = table.Column<int>(nullable: true),
                    DwellingStartDate = table.Column<DateTime>(nullable: true),
                    CaptureStartDate = table.Column<DateTime>(nullable: true),
                    CaptureEndDate = table.Column<DateTime>(nullable: true),
                    Longitude = table.Column<float>(nullable: true),
                    Latitude = table.Column<float>(nullable: true),
                    Elevation = table.Column<float>(nullable: true),
                    SerialNo = table.Column<string>(nullable: true),
                    DeviceId = table.Column<string>(nullable: true),
                    VersionId = table.Column<string>(nullable: true),
                    SyncDate = table.Column<DateTime>(nullable: true),
                    CommonName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    RecipientNames = table.Column<string>(nullable: true),
                    OwnsPhone = table.Column<bool>(nullable: false),
                    NomineeFirstName = table.Column<string>(nullable: true),
                    NomineeMiddleName = table.Column<string>(nullable: true),
                    NomineeSurname = table.Column<string>(nullable: true),
                    NomineeIdNumber = table.Column<string>(nullable: true),
                    NOKFirstName = table.Column<string>(nullable: true),
                    NOKMiddleName = table.Column<string>(nullable: true),
                    NOKSurname = table.Column<string>(nullable: true),
                    NOKPhone = table.Column<string>(maxLength: 10, nullable: true),
                    NOKIdNumber = table.Column<string>(nullable: true),
                    HasProxy = table.Column<bool>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: false),
                    SupportingDocument = table.Column<string>(nullable: true),
                    Institution = table.Column<string>(nullable: true),
                    PhotoUrl = table.Column<string>(nullable: true),
                    NHIFNo = table.Column<string>(nullable: true),
                    Para = table.Column<string>(nullable: true),
                    Gravida = table.Column<int>(nullable: true),
                    Parity = table.Column<string>(nullable: true),
                    LMP = table.Column<DateTime>(nullable: true),
                    LMPUnknown = table.Column<bool>(nullable: true),
                    EPM = table.Column<int>(nullable: true),
                    EDD = table.Column<DateTime>(nullable: true),
                    FingerPrint = table.Column<byte[]>(nullable: true),
                    RawFingerPrint = table.Column<byte[]>(nullable: true),
                    VerifyingFingerPrint = table.Column<bool>(nullable: false),
                    MotherId = table.Column<string>(nullable: true),
                    ProgrammeId = table.Column<byte>(nullable: true),
                    PMTScore = table.Column<decimal>(type: "decimal(18, 10)", nullable: true),
                    PMTScoreFinal = table.Column<decimal>(type: "decimal(18, 10)", nullable: true),
                    LCS = table.Column<decimal>(type: "decimal(18, 10)", nullable: true),
                    LCSFinal = table.Column<decimal>(type: "decimal(18, 10)", nullable: true),
                    TypeId = table.Column<int>(nullable: true),
                    ParentId = table.Column<string>(nullable: true),
                    RequiresHealthVerification = table.Column<bool>(nullable: true),
                    RequiresIPRSECheck = table.Column<bool>(nullable: false),
                    IPRSVerified = table.Column<bool>(nullable: false),
                    IPRSPassed = table.Column<bool>(nullable: true),
                    IPRSExceptionId = table.Column<int>(nullable: true),
                    ExitReasonId = table.Column<int>(nullable: true),
                    RequiresMPESACheck = table.Column<bool>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: false),
                    DateApproved = table.Column<DateTime>(nullable: true),
                    ApprovedById = table.Column<string>(nullable: true),
                    BlackListReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdRegs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_CommunityAreas_CommunityAreaId",
                        column: x => x.CommunityAreaId,
                        principalTable: "CommunityAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_SystemCodeDetails_ExitReasonId",
                        column: x => x.ExitReasonId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_HealthFacilities_HealthFacilityId",
                        column: x => x.HealthFacilityId,
                        principalTable: "HealthFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_SystemCodeDetails_IPRSExceptionId",
                        column: x => x.IPRSExceptionId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_Counties_OtherCountyId",
                        column: x => x.OtherCountyId,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_Programmes_ProgrammeId",
                        column: x => x.ProgrammeId,
                        principalTable: "Programmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_Reasons_ReasonId",
                        column: x => x.ReasonId,
                        principalTable: "Reasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_SubLocations_SubLocationId",
                        column: x => x.SubLocationId,
                        principalTable: "SubLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_Villages_VillageId",
                        column: x => x.VillageId,
                        principalTable: "Villages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegs_Wards_WardId",
                        column: x => x.WardId,
                        principalTable: "Wards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnrolmentDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EnrolmentId = table.Column<int>(nullable: false),
                    HouseholdId = table.Column<string>(nullable: true),
                    RecipientNames = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    RecipientPhone = table.Column<string>(nullable: true),
                    CustomerType = table.Column<string>(nullable: true),
                    HasProxy = table.Column<bool>(nullable: false),
                    StatusId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrolmentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrolmentDetails_Enrolments_EnrolmentId",
                        column: x => x.EnrolmentId,
                        principalTable: "Enrolments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EnrolmentDetails_HouseholdRegs_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "HouseholdRegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EnrolmentDetails_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdRegAssets",
                columns: table => new
                {
                    HouseholdId = table.Column<string>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    AssetTypeId = table.Column<int>(nullable: false),
                    HasItem = table.Column<bool>(nullable: true),
                    ItemCount = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdRegAssets", x => new { x.HouseholdId, x.AssetId });
                    table.UniqueConstraint("AK_HouseholdRegAssets_AssetId_HouseholdId", x => new { x.AssetId, x.HouseholdId });
                    table.ForeignKey(
                        name: "FK_HouseholdRegAssets_SystemCodeDetails_AssetId",
                        column: x => x.AssetId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegAssets_SystemCodeDetails_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegAssets_HouseholdRegs_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "HouseholdRegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdRegMembers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    HouseholdId = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    IdentificationFormId = table.Column<int>(nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    RelationshipId = table.Column<int>(nullable: true),
                    GenderId = table.Column<int>(nullable: true),
                    DOB = table.Column<DateTime>(nullable: true),
                    MaritalStatusId = table.Column<int>(nullable: true),
                    SpouseInHouseholdId = table.Column<int>(nullable: true),
                    FatherAliveId = table.Column<int>(nullable: true),
                    MotherAliveId = table.Column<int>(nullable: true),
                    ChronicIllnessId = table.Column<int>(nullable: true),
                    DisabilityCaregiverId = table.Column<string>(nullable: true),
                    DisabilityRequires24HrCareId = table.Column<int>(nullable: true),
                    ExternalMember = table.Column<bool>(nullable: true),
                    MainCaregiverId = table.Column<int>(nullable: true),
                    EducationAttendanceId = table.Column<int>(nullable: true),
                    EducationLevelId = table.Column<int>(nullable: true),
                    SchoolTypeId = table.Column<int>(nullable: true),
                    OccupationTypeId = table.Column<int>(nullable: true),
                    FormalJobTypeId = table.Column<int>(nullable: true),
                    SupportStatusId = table.Column<string>(nullable: true),
                    MUAC = table.Column<decimal>(nullable: true),
                    CreateOn = table.Column<DateTime>(nullable: false),
                    OrphanhoodTypeId = table.Column<int>(nullable: true),
                    IllnessTypeId = table.Column<int>(nullable: true),
                    DisabilityTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdRegMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_ChronicIllnessId",
                        column: x => x.ChronicIllnessId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_HouseholdRegMembers_DisabilityCaregiverId",
                        column: x => x.DisabilityCaregiverId,
                        principalTable: "HouseholdRegMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_DisabilityRequires24HrCareId",
                        column: x => x.DisabilityRequires24HrCareId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_DisabilityTypeId",
                        column: x => x.DisabilityTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_EducationAttendanceId",
                        column: x => x.EducationAttendanceId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_EducationLevelId",
                        column: x => x.EducationLevelId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_FatherAliveId",
                        column: x => x.FatherAliveId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_FormalJobTypeId",
                        column: x => x.FormalJobTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_GenderId",
                        column: x => x.GenderId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_HouseholdRegs_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "HouseholdRegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_IdentificationFormId",
                        column: x => x.IdentificationFormId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_IllnessTypeId",
                        column: x => x.IllnessTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_MaritalStatusId",
                        column: x => x.MaritalStatusId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_MotherAliveId",
                        column: x => x.MotherAliveId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_OccupationTypeId",
                        column: x => x.OccupationTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_OrphanhoodTypeId",
                        column: x => x.OrphanhoodTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_RelationshipId",
                        column: x => x.RelationshipId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_SchoolTypeId",
                        column: x => x.SchoolTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMembers_SystemCodeDetails_SpouseInHouseholdId",
                        column: x => x.SpouseInHouseholdId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdRegOtherProgrammes",
                columns: table => new
                {
                    HouseholdId = table.Column<string>(nullable: false),
                    OtherProgrammeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdRegOtherProgrammes", x => new { x.HouseholdId, x.OtherProgrammeId });
                    table.ForeignKey(
                        name: "FK_HouseholdRegOtherProgrammes_HouseholdRegs_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "HouseholdRegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegOtherProgrammes_SystemCodeDetails_OtherProgrammeId",
                        column: x => x.OtherProgrammeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false),
                    HouseholdId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_NotesCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "NotesCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notes_HouseholdRegs_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "HouseholdRegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PMTScores",
                columns: table => new
                {
                    HouseholdId = table.Column<string>(nullable: false),
                    ConstantId = table.Column<int>(nullable: false),
                    Score = table.Column<float>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    LocalityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMTScores", x => x.HouseholdId);
                    table.ForeignKey(
                        name: "FK_PMTScores_SystemCodeDetails_ConstantId",
                        column: x => x.ConstantId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PMTScores_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PMTScores_HouseholdRegs_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "HouseholdRegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PMTScores_SystemCodeDetails_LocalityId",
                        column: x => x.LocalityId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    EnrolmentId = table.Column<int>(nullable: true),
                    HouseholdId = table.Column<string>(nullable: true),
                    UniqueId = table.Column<string>(nullable: true),
                    BeneficiaryName = table.Column<string>(nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    IdentificationFormId = table.Column<int>(nullable: true),
                    RecipientName = table.Column<string>(nullable: true),
                    RecipientPhone = table.Column<string>(nullable: true),
                    EDD = table.Column<DateTime>(nullable: true),
                    DOB = table.Column<DateTime>(nullable: false),
                    VillageId = table.Column<int>(nullable: true),
                    CommunityAreaId = table.Column<int>(nullable: true),
                    StatusId = table.Column<int>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: false),
                    DateEnrolled = table.Column<DateTime>(nullable: true),
                    MotherId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_CommunityAreas_CommunityAreaId",
                        column: x => x.CommunityAreaId,
                        principalTable: "CommunityAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_Enrolments_EnrolmentId",
                        column: x => x.EnrolmentId,
                        principalTable: "Enrolments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_HealthFacilities_HealthFacilityId",
                        column: x => x.HealthFacilityId,
                        principalTable: "HealthFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_HouseholdRegs_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "HouseholdRegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_SystemCodeDetails_IdentificationFormId",
                        column: x => x.IdentificationFormId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_HouseholdRegMembers_MotherId",
                        column: x => x.MotherId,
                        principalTable: "HouseholdRegMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_Villages_VillageId",
                        column: x => x.VillageId,
                        principalTable: "Villages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdRegMemberDisabilities",
                columns: table => new
                {
                    HouseholdRegMemberId = table.Column<string>(nullable: false),
                    DisabilityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdRegMemberDisabilities", x => new { x.HouseholdRegMemberId, x.DisabilityId });
                    table.UniqueConstraint("AK_HouseholdRegMemberDisabilities_DisabilityId_HouseholdRegMemberId", x => new { x.DisabilityId, x.HouseholdRegMemberId });
                    table.ForeignKey(
                        name: "FK_HouseholdRegMemberDisabilities_SystemCodeDetails_DisabilityId",
                        column: x => x.DisabilityId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseholdRegMemberDisabilities_HouseholdRegMembers_HouseholdRegMemberId",
                        column: x => x.HouseholdRegMemberId,
                        principalTable: "HouseholdRegMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayrollId = table.Column<int>(nullable: false),
                    FundRequestId = table.Column<int>(nullable: false),
                    CycleId = table.Column<int>(nullable: false),
                    BeneficiaryId = table.Column<string>(nullable: true),
                    BeneficiaryName = table.Column<string>(nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    RecipientName = table.Column<string>(nullable: true),
                    RecipientPhone = table.Column<string>(nullable: true),
                    VillageId = table.Column<int>(nullable: true),
                    WardId = table.Column<int>(nullable: true),
                    SubCountyId = table.Column<int>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    FailureReason = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Reconciled = table.Column<bool>(nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: true),
                    ReceiverPartyPublicName = table.Column<string>(nullable: true),
                    TransactionReceipt = table.Column<string>(nullable: true),
                    PaymentErrorMessage = table.Column<string>(nullable: true),
                    TransactionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Beneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "Beneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentCycles_CycleId",
                        column: x => x.CycleId,
                        principalTable: "PaymentCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_FundRequests_FundRequestId",
                        column: x => x.FundRequestId,
                        principalTable: "FundRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_HealthFacilities_HealthFacilityId",
                        column: x => x.HealthFacilityId,
                        principalTable: "HealthFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Payrolls_PayrollId",
                        column: x => x.PayrollId,
                        principalTable: "Payrolls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "PaymentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_SubCounties_SubCountyId",
                        column: x => x.SubCountyId,
                        principalTable: "SubCounties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Villages_VillageId",
                        column: x => x.VillageId,
                        principalTable: "Villages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Wards_WardId",
                        column: x => x.WardId,
                        principalTable: "Wards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BeneficiaryId = table.Column<string>(nullable: true),
                    PaymentPointId = table.Column<int>(nullable: false),
                    BeneficiaryPaymentPointId = table.Column<int>(nullable: true),
                    FundRequestId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    BeneficiaryName = table.Column<string>(nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    RecipientName = table.Column<string>(nullable: true),
                    RecipientPhone = table.Column<string>(nullable: true),
                    VillageId = table.Column<int>(nullable: true),
                    WardId = table.Column<int>(nullable: true),
                    SubCountyId = table.Column<int>(nullable: true),
                    HealthFacilityId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Beneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "Beneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_FundRequests_FundRequestId",
                        column: x => x.FundRequestId,
                        principalTable: "FundRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_HealthFacilities_HealthFacilityId",
                        column: x => x.HealthFacilityId,
                        principalTable: "HealthFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_PaymentPoints_PaymentPointId",
                        column: x => x.PaymentPointId,
                        principalTable: "PaymentPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_PaymentStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "PaymentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_SubCounties_SubCountyId",
                        column: x => x.SubCountyId,
                        principalTable: "SubCounties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Villages_VillageId",
                        column: x => x.VillageId,
                        principalTable: "Villages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Wards_WardId",
                        column: x => x.WardId,
                        principalTable: "Wards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CreatedById",
                table: "AspNetUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_HealthFacilityId",
                table: "AspNetUsers",
                column: "HealthFacilityId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SubCountyId",
                table: "AspNetUsers",
                column: "SubCountyId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrail_UserId",
                table: "AuditTrail",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_CommunityAreaId",
                table: "Beneficiaries",
                column: "CommunityAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_EnrolmentId",
                table: "Beneficiaries",
                column: "EnrolmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_HealthFacilityId",
                table: "Beneficiaries",
                column: "HealthFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_HouseholdId",
                table: "Beneficiaries",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_IdentificationFormId",
                table: "Beneficiaries",
                column: "IdentificationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_MotherId",
                table: "Beneficiaries",
                column: "MotherId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_StatusId",
                table: "Beneficiaries",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_VillageId",
                table: "Beneficiaries",
                column: "VillageId");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryPaymentPoints_CreatedById",
                table: "BeneficiaryPaymentPoints",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryPaymentPoints_HouseholdId",
                table: "BeneficiaryPaymentPoints",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryPaymentPoints_PaymentPointId",
                table: "BeneficiaryPaymentPoints",
                column: "PaymentPointId");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryPaymentPoints_StatusId",
                table: "BeneficiaryPaymentPoints",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseManagement_CreatedById",
                table: "CaseManagement",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CaseManagement_HouseholdId",
                table: "CaseManagement",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseManagement_ReasonId",
                table: "CaseManagement",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseManagement_StatusId",
                table: "CaseManagement",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeNotes_ChangeId",
                table: "ChangeNotes",
                column: "ChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeNotes_CreatedById",
                table: "ChangeNotes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Changes_ActionedById",
                table: "Changes",
                column: "ActionedById");

            migrationBuilder.CreateIndex(
                name: "IX_Changes_ChangeTypeId",
                table: "Changes",
                column: "ChangeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Changes_ChildId",
                table: "Changes",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Changes_CreatedById",
                table: "Changes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Changes_HouseholdId",
                table: "Changes",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_Changes_MPESACheckStatusId",
                table: "Changes",
                column: "MPESACheckStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Changes_StatusId",
                table: "Changes",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildFeedingInformation_BreastfeedingId1",
                table: "ChildFeedingInformation",
                column: "BreastfeedingId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChildFeedingInformation_ComplimentaryFoodIntroducedId1",
                table: "ChildFeedingInformation",
                column: "ComplimentaryFoodIntroducedId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChildFeedingInformation_OtherFoodIntroducedId1",
                table: "ChildFeedingInformation",
                column: "OtherFoodIntroducedId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChildFeedingInformation_PregnancyId",
                table: "ChildFeedingInformation",
                column: "PregnancyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildHealthRecords_CreatedById",
                table: "ChildHealthRecords",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChildHealthRecords_DeliveryPlaceId",
                table: "ChildHealthRecords",
                column: "DeliveryPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildHealthRecords_HealthFacilityId",
                table: "ChildHealthRecords",
                column: "HealthFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildHealthRecords_PregnancyId",
                table: "ChildHealthRecords",
                column: "PregnancyId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_DeliveryAssistantId",
                table: "Children",
                column: "DeliveryAssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_DeliveryHealthFacilityId",
                table: "Children",
                column: "DeliveryHealthFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_DeliveryId",
                table: "Children",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_DeliveryPlaceId",
                table: "Children",
                column: "DeliveryPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_GenderId",
                table: "Children",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_ImmunizedId",
                table: "Children",
                column: "ImmunizedId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_StatusId",
                table: "Children",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CivilRegistrations_PregnancyId",
                table: "CivilRegistrations",
                column: "PregnancyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicVisits_PaymentPointId",
                table: "ClinicVisits",
                column: "PaymentPointId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicVisits_VisitTypeId",
                table: "ClinicVisits",
                column: "VisitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAreas_VillageId",
                table: "CommunityAreas",
                column: "VillageId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintNotes_CategoryId",
                table: "ComplaintNotes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintNotes_ComplaintId",
                table: "ComplaintNotes",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintNotes_CreatedById",
                table: "ComplaintNotes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ApprovedById",
                table: "Complaints",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_CategoryId",
                table: "Complaints",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ClosedById",
                table: "Complaints",
                column: "ClosedById");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ComplaintTypeId",
                table: "Complaints",
                column: "ComplaintTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_CreatedById",
                table: "Complaints",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_EscalatedById",
                table: "Complaints",
                column: "EscalatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_HealthFacilityId",
                table: "Complaints",
                column: "HealthFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_IsComplainantAnonymousId",
                table: "Complaints",
                column: "IsComplainantAnonymousId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_IsComplainantBeneficiaryId",
                table: "Complaints",
                column: "IsComplainantBeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ResolvedById",
                table: "Complaints",
                column: "ResolvedById");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_StatusId",
                table: "Complaints",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_VillageId",
                table: "Complaints",
                column: "VillageId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintTypes_CategoryId",
                table: "ComplaintTypes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Constituencies_CountyId",
                table: "Constituencies",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_Counties_CreatedById",
                table: "Counties",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Counties_GeoMasterId",
                table: "Counties",
                column: "GeoMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Counties_ModifiedById",
                table: "Counties",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_CutOffs_SubLocationId1",
                table: "CutOffs",
                column: "SubLocationId1");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_ActionedById",
                table: "CVListDetails",
                column: "ActionedById");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_ApprovalStatusId",
                table: "CVListDetails",
                column: "ApprovalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_CVHouseHoldId",
                table: "CVListDetails",
                column: "CVHouseHoldId");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_CannotFindHouseholdReasonId",
                table: "CVListDetails",
                column: "CannotFindHouseholdReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_EnumeratorId",
                table: "CVListDetails",
                column: "EnumeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_HouseholdId",
                table: "CVListDetails",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_InterviewResultId",
                table: "CVListDetails",
                column: "InterviewResultId");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_InterviewStatusId",
                table: "CVListDetails",
                column: "InterviewStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_ListId",
                table: "CVListDetails",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_StatusId",
                table: "CVListDetails",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CVListDetails_VarianceCategoryId",
                table: "CVListDetails",
                column: "VarianceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CVLists_ApprovedById",
                table: "CVLists",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_CVLists_CreatedById",
                table: "CVLists",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CVLists_ListTypeId",
                table: "CVLists",
                column: "ListTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CVLists_StatusId",
                table: "CVLists",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_BloodLossId",
                table: "Deliveries",
                column: "BloodLossId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_CreatedById",
                table: "Deliveries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryModeId",
                table: "Deliveries",
                column: "DeliveryModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_HIVTestedId",
                table: "Deliveries",
                column: "HIVTestedId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_MeconiumStainedLiquorId",
                table: "Deliveries",
                column: "MeconiumStainedLiquorId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_MotherClinicVisitId",
                table: "Deliveries",
                column: "MotherClinicVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_ObstructedLabourId",
                table: "Deliveries",
                column: "ObstructedLabourId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_PregnancyId",
                table: "Deliveries",
                column: "PregnancyId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_PregnancyOutcomeId",
                table: "Deliveries",
                column: "PregnancyOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_RescuscitationDoneId",
                table: "Deliveries",
                column: "RescuscitationDoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_SupportStatusId",
                table: "Deliveries",
                column: "SupportStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ConstituencyId",
                table: "Districts",
                column: "ConstituencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_GeoMasterId",
                table: "Districts",
                column: "GeoMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_CreatedById",
                table: "Divisions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_DistrictId",
                table: "Divisions",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_ModifiedById",
                table: "Divisions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_DrugsAdministered_DrugId",
                table: "DrugsAdministered",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolmentDetails_EnrolmentId",
                table: "EnrolmentDetails",
                column: "EnrolmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolmentDetails_HouseholdId",
                table: "EnrolmentDetails",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolmentDetails_StatusId",
                table: "EnrolmentDetails",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_ApprovedById",
                table: "Enrolments",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_CreatedById",
                table: "Enrolments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_ImportApprovedById",
                table: "Enrolments",
                column: "ImportApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_ImportStatusId",
                table: "Enrolments",
                column: "ImportStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_ImportedById",
                table: "Enrolments",
                column: "ImportedById");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_StatusId",
                table: "Enrolments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Enumerators_EnumeratorGroupId",
                table: "Enumerators",
                column: "EnumeratorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Enumerators_VillageId",
                table: "Enumerators",
                column: "VillageId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyPlannings_CreatedById",
                table: "FamilyPlannings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyPlannings_PregnancyId",
                table: "FamilyPlannings",
                column: "PregnancyId");

            migrationBuilder.CreateIndex(
                name: "IX_FingerPrintVerifications_CreatedById",
                table: "FingerPrintVerifications",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FundRequests_ApprovedById",
                table: "FundRequests",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_FundRequests_CreatedById",
                table: "FundRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FundRequests_CycleId",
                table: "FundRequests",
                column: "CycleId");

            migrationBuilder.CreateIndex(
                name: "IX_FundRequests_StatusId",
                table: "FundRequests",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GeoMaster_CreatedById",
                table: "GeoMaster",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GeoMaster_ModifiedById",
                table: "GeoMaster",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_HealthFacilities_SubCountyId",
                table: "HealthFacilities",
                column: "SubCountyId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegAssets_AssetTypeId",
                table: "HouseholdRegAssets",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_BenefitFromFriendsRelativeId",
                table: "HouseholdRegCharacteristics",
                column: "BenefitFromFriendsRelativeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_CookingFuelSourceId",
                table: "HouseholdRegCharacteristics",
                column: "CookingFuelSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_FloorMaterialId",
                table: "HouseholdRegCharacteristics",
                column: "FloorMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_HasSkippedMealId",
                table: "HouseholdRegCharacteristics",
                column: "HasSkippedMealId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_HouseholdConditionId",
                table: "HouseholdRegCharacteristics",
                column: "HouseholdConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_IsReceivingOtherBenefitId",
                table: "HouseholdRegCharacteristics",
                column: "IsReceivingOtherBenefitId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_LightingSourceId",
                table: "HouseholdRegCharacteristics",
                column: "LightingSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_OtherProgrammesBenefitTypeId",
                table: "HouseholdRegCharacteristics",
                column: "OtherProgrammesBenefitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_RoofMaterialId",
                table: "HouseholdRegCharacteristics",
                column: "RoofMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_TenureOwnerOccupiedId",
                table: "HouseholdRegCharacteristics",
                column: "TenureOwnerOccupiedId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_TenureStatusId",
                table: "HouseholdRegCharacteristics",
                column: "TenureStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_ToiletTypeId",
                table: "HouseholdRegCharacteristics",
                column: "ToiletTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_UnitRiskId",
                table: "HouseholdRegCharacteristics",
                column: "UnitRiskId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_WallMaterialId",
                table: "HouseholdRegCharacteristics",
                column: "WallMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegCharacteristics_WaterSourceId",
                table: "HouseholdRegCharacteristics",
                column: "WaterSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_ChronicIllnessId",
                table: "HouseholdRegMembers",
                column: "ChronicIllnessId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_DisabilityCaregiverId",
                table: "HouseholdRegMembers",
                column: "DisabilityCaregiverId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_DisabilityRequires24HrCareId",
                table: "HouseholdRegMembers",
                column: "DisabilityRequires24HrCareId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_DisabilityTypeId",
                table: "HouseholdRegMembers",
                column: "DisabilityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_EducationAttendanceId",
                table: "HouseholdRegMembers",
                column: "EducationAttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_EducationLevelId",
                table: "HouseholdRegMembers",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_FatherAliveId",
                table: "HouseholdRegMembers",
                column: "FatherAliveId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_FormalJobTypeId",
                table: "HouseholdRegMembers",
                column: "FormalJobTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_GenderId",
                table: "HouseholdRegMembers",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_HouseholdId",
                table: "HouseholdRegMembers",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_IdentificationFormId",
                table: "HouseholdRegMembers",
                column: "IdentificationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_IllnessTypeId",
                table: "HouseholdRegMembers",
                column: "IllnessTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_MaritalStatusId",
                table: "HouseholdRegMembers",
                column: "MaritalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_MotherAliveId",
                table: "HouseholdRegMembers",
                column: "MotherAliveId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_OccupationTypeId",
                table: "HouseholdRegMembers",
                column: "OccupationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_OrphanhoodTypeId",
                table: "HouseholdRegMembers",
                column: "OrphanhoodTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_RelationshipId",
                table: "HouseholdRegMembers",
                column: "RelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_SchoolTypeId",
                table: "HouseholdRegMembers",
                column: "SchoolTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegMembers_SpouseInHouseholdId",
                table: "HouseholdRegMembers",
                column: "SpouseInHouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegOtherProgrammes_OtherProgrammeId",
                table: "HouseholdRegOtherProgrammes",
                column: "OtherProgrammeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_ApprovedById",
                table: "HouseholdRegs",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_CommunityAreaId",
                table: "HouseholdRegs",
                column: "CommunityAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_CountyId",
                table: "HouseholdRegs",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_ExitReasonId",
                table: "HouseholdRegs",
                column: "ExitReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_HealthFacilityId",
                table: "HouseholdRegs",
                column: "HealthFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_IPRSExceptionId",
                table: "HouseholdRegs",
                column: "IPRSExceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_MotherId",
                table: "HouseholdRegs",
                column: "MotherId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_OtherCountyId",
                table: "HouseholdRegs",
                column: "OtherCountyId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_ProgrammeId",
                table: "HouseholdRegs",
                column: "ProgrammeId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_ReasonId",
                table: "HouseholdRegs",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_StatusId",
                table: "HouseholdRegs",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_SubLocationId",
                table: "HouseholdRegs",
                column: "SubLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_VillageId",
                table: "HouseholdRegs",
                column: "VillageId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_WardId",
                table: "HouseholdRegs",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdRegs_CreatedById_CommunityAreaId_VillageId_StatusId_HealthFacilityId_IPRSExceptionId_MotherId_WardId",
                table: "HouseholdRegs",
                columns: new[] { "CreatedById", "CommunityAreaId", "VillageId", "StatusId", "HealthFacilityId", "IPRSExceptionId", "MotherId", "WardId" });

            migrationBuilder.CreateIndex(
                name: "IX_InstallationSetup_HealthFacilityId",
                table: "InstallationSetup",
                column: "HealthFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyMilestones_ClinicVisitId",
                table: "KeyMilestones",
                column: "ClinicVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CreatedById",
                table: "Locations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_DivisionId",
                table: "Locations",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ModifiedById",
                table: "Locations",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_MotherClinicVisits_CaseManagementId",
                table: "MotherClinicVisits",
                column: "CaseManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_MotherClinicVisits_ClinicVisitId",
                table: "MotherClinicVisits",
                column: "ClinicVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_MotherClinicVisits_CreatedById",
                table: "MotherClinicVisits",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MotherClinicVisits_PregnancyId",
                table: "MotherClinicVisits",
                column: "PregnancyId");

            migrationBuilder.CreateIndex(
                name: "IX_MotherPreventiveServices_PreventiveServiceId",
                table: "MotherPreventiveServices",
                column: "PreventiveServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_CategoryId",
                table: "Notes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_CreatedById",
                table: "Notes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_HouseholdId",
                table: "Notes",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCycles_CreatedById",
                table: "PaymentCycles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BeneficiaryId",
                table: "Payments",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CycleId",
                table: "Payments",
                column: "CycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_FundRequestId",
                table: "Payments",
                column: "FundRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_HealthFacilityId",
                table: "Payments",
                column: "HealthFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StatusId",
                table: "Payments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SubCountyId",
                table: "Payments",
                column: "SubCountyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_VillageId",
                table: "Payments",
                column: "VillageId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_WardId",
                table: "Payments",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PayrollId_CycleId_BeneficiaryId_StatusId_HealthFacilityId_VillageId_WardId_SubCountyId",
                table: "Payments",
                columns: new[] { "PayrollId", "CycleId", "BeneficiaryId", "StatusId", "HealthFacilityId", "VillageId", "WardId", "SubCountyId" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_BeneficiaryId",
                table: "PaymentTransactions",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_FundRequestId",
                table: "PaymentTransactions",
                column: "FundRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_HealthFacilityId",
                table: "PaymentTransactions",
                column: "HealthFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentPointId",
                table: "PaymentTransactions",
                column: "PaymentPointId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_StatusId",
                table: "PaymentTransactions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_SubCountyId",
                table: "PaymentTransactions",
                column: "SubCountyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_VillageId",
                table: "PaymentTransactions",
                column: "VillageId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_WardId",
                table: "PaymentTransactions",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollExceptions_ExceptionTypeId",
                table: "PayrollExceptions",
                column: "ExceptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_ApprovedById",
                table: "Payrolls",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_CreatedById",
                table: "Payrolls",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_CycleId",
                table: "Payrolls",
                column: "CycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_FundRequestId",
                table: "Payrolls",
                column: "FundRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_ReconciledById",
                table: "Payrolls",
                column: "ReconciledById");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_ReconciliationApprovedById",
                table: "Payrolls",
                column: "ReconciliationApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_ReconciliationStatusId",
                table: "Payrolls",
                column: "ReconciliationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_StatusId",
                table: "Payrolls",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PMTCTServices_CreatedById",
                table: "PMTCTServices",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PMTCTServices_PregnancyId",
                table: "PMTCTServices",
                column: "PregnancyId");

            migrationBuilder.CreateIndex(
                name: "IX_PMTScores_ConstantId",
                table: "PMTScores",
                column: "ConstantId");

            migrationBuilder.CreateIndex(
                name: "IX_PMTScores_CreatedBy",
                table: "PMTScores",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PMTScores_LocalityId",
                table: "PMTScores",
                column: "LocalityId");

            migrationBuilder.CreateIndex(
                name: "IX_PMTWeights_LocalityId",
                table: "PMTWeights",
                column: "LocalityId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminationDetails_ARTProphylaxisGivenId",
                table: "PostNatalExaminationDetails",
                column: "ARTProphylaxisGivenId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminationDetails_CPInitiatedId",
                table: "PostNatalExaminationDetails",
                column: "CPInitiatedId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminationDetails_ChildId",
                table: "PostNatalExaminationDetails",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminationDetails_FeedingMethodId",
                table: "PostNatalExaminationDetails",
                column: "FeedingMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminationDetails_ImmunizationStartedId",
                table: "PostNatalExaminationDetails",
                column: "ImmunizationStartedId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminationDetails_PostNatalExaminationId",
                table: "PostNatalExaminationDetails",
                column: "PostNatalExaminationId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminations_ClinicVisitId",
                table: "PostNatalExaminations",
                column: "ClinicVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminations_CreatedById",
                table: "PostNatalExaminations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminations_FPCounselingId",
                table: "PostNatalExaminations",
                column: "FPCounselingId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminations_FPMethodId",
                table: "PostNatalExaminations",
                column: "FPMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminations_MotherClinicVisitId",
                table: "PostNatalExaminations",
                column: "MotherClinicVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminations_PregnancyId",
                table: "PostNatalExaminations",
                column: "PregnancyId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNatalExaminations_SupportStatusId",
                table: "PostNatalExaminations",
                column: "SupportStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancies_BloodGroupId",
                table: "Pregnancies",
                column: "BloodGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancies_BreastfeedingCounselingDoneId",
                table: "Pregnancies",
                column: "BreastfeedingCounselingDoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancies_CaseManagementId",
                table: "Pregnancies",
                column: "CaseManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancies_CreatedById",
                table: "Pregnancies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancies_InfantFeedingCounselingDoneId",
                table: "Pregnancies",
                column: "InfantFeedingCounselingDoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancies_ModifiedById",
                table: "Pregnancies",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancies_ReasonId",
                table: "Pregnancies",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancies_RhesusId",
                table: "Pregnancies",
                column: "RhesusId");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancies_StatusId",
                table: "Pregnancies",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancies_SupportStatusId",
                table: "Pregnancies",
                column: "SupportStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PregnancyData_ClinicVisitId",
                table: "PregnancyData",
                column: "ClinicVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PregnancyData_CreatedById",
                table: "PregnancyData",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PregnancyData_MotherClinicVisitId",
                table: "PregnancyData",
                column: "MotherClinicVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PregnancyData_PregnancyId",
                table: "PregnancyData",
                column: "PregnancyId");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecks_ApprovedById",
                table: "PrePayrollChecks",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecks_CreatedById",
                table: "PrePayrollChecks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecks_PaymentCycleId",
                table: "PrePayrollChecks",
                column: "PaymentCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecks_StatusId",
                table: "PrePayrollChecks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecksDetails_ActionId",
                table: "PrePayrollChecksDetails",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecksDetails_ApprovedById",
                table: "PrePayrollChecksDetails",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecksDetails_BeneficiaryId",
                table: "PrePayrollChecksDetails",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecksDetails_ExceptionId",
                table: "PrePayrollChecksDetails",
                column: "ExceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecksDetails_PaymentId",
                table: "PrePayrollChecksDetails",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecksDetails_PrePayrollCheckId",
                table: "PrePayrollChecksDetails",
                column: "PrePayrollCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollChecksDetails_StatusId",
                table: "PrePayrollChecksDetails",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PrePayrollTransactions_PaymentPointId",
                table: "PrePayrollTransactions",
                column: "PaymentPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportCategoryId",
                table: "Reports",
                column: "ReportCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SMS_ClinicVisitId",
                table: "SMS",
                column: "ClinicVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCounties_CountyId",
                table: "SubCounties",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_SubLocations_CreatedById",
                table: "SubLocations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubLocations_LocationId",
                table: "SubLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SubLocations_ModifiedById",
                table: "SubLocations",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SystemCodeDetails_SystemCodeId",
                table: "SystemCodeDetails",
                column: "SystemCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemCodes_SystemModuleId",
                table: "SystemCodes",
                column: "SystemModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemTasks_ParentId",
                table: "SystemTasks",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Villages_SubLocationId",
                table: "Villages",
                column: "SubLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Villages_WardId",
                table: "Villages",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_ConstituencyId",
                table: "Wards",
                column: "ConstituencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_SubCountyId",
                table: "Wards",
                column: "SubCountyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Changes_AspNetUsers_ActionedById",
                table: "Changes",
                column: "ActionedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Changes_AspNetUsers_CreatedById",
                table: "Changes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Changes_HouseholdRegs_HouseholdId",
                table: "Changes",
                column: "HouseholdId",
                principalTable: "HouseholdRegs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Changes_Children_ChildId",
                table: "Changes",
                column: "ChildId",
                principalTable: "Children",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CVListDetails_AspNetUsers_ActionedById",
                table: "CVListDetails",
                column: "ActionedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CVListDetails_HouseholdRegs_CVHouseHoldId",
                table: "CVListDetails",
                column: "CVHouseHoldId",
                principalTable: "HouseholdRegs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CVListDetails_HouseholdRegs_HouseholdId",
                table: "CVListDetails",
                column: "HouseholdId",
                principalTable: "HouseholdRegs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CVListDetails_Enumerators_EnumeratorId",
                table: "CVListDetails",
                column: "EnumeratorId",
                principalTable: "Enumerators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CVListDetails_CVLists_ListId",
                table: "CVListDetails",
                column: "ListId",
                principalTable: "CVLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CVLists_AspNetUsers_ApprovedById",
                table: "CVLists",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CVLists_AspNetUsers_CreatedById",
                table: "CVLists",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrolments_AspNetUsers_ApprovedById",
                table: "Enrolments",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrolments_AspNetUsers_CreatedById",
                table: "Enrolments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrolments_AspNetUsers_ImportApprovedById",
                table: "Enrolments",
                column: "ImportApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrolments_AspNetUsers_ImportedById",
                table: "Enrolments",
                column: "ImportedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FundRequests_AspNetUsers_ApprovedById",
                table: "FundRequests",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FundRequests_AspNetUsers_CreatedById",
                table: "FundRequests",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FundRequests_PaymentCycles_CycleId",
                table: "FundRequests",
                column: "CycleId",
                principalTable: "PaymentCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_AspNetUsers_ApprovedById",
                table: "Payrolls",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_AspNetUsers_CreatedById",
                table: "Payrolls",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_AspNetUsers_ReconciledById",
                table: "Payrolls",
                column: "ReconciledById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_AspNetUsers_ReconciliationApprovedById",
                table: "Payrolls",
                column: "ReconciliationApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_PaymentCycles_CycleId",
                table: "Payrolls",
                column: "CycleId",
                principalTable: "PaymentCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrePayrollChecks_AspNetUsers_ApprovedById",
                table: "PrePayrollChecks",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrePayrollChecks_AspNetUsers_CreatedById",
                table: "PrePayrollChecks",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrePayrollChecks_PaymentCycles_PaymentCycleId",
                table: "PrePayrollChecks",
                column: "PaymentCycleId",
                principalTable: "PaymentCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrePayrollChecksDetails_AspNetUsers_ApprovedById",
                table: "PrePayrollChecksDetails",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrePayrollChecksDetails_Beneficiaries_BeneficiaryId",
                table: "PrePayrollChecksDetails",
                column: "BeneficiaryId",
                principalTable: "Beneficiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrePayrollChecksDetails_Payments_PaymentId",
                table: "PrePayrollChecksDetails",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditTrail_AspNetUsers_UserId",
                table: "AuditTrail",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BeneficiaryPaymentPoints_AspNetUsers_CreatedById",
                table: "BeneficiaryPaymentPoints",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BeneficiaryPaymentPoints_HouseholdRegs_HouseholdId",
                table: "BeneficiaryPaymentPoints",
                column: "HouseholdId",
                principalTable: "HouseholdRegs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseManagement_AspNetUsers_CreatedById",
                table: "CaseManagement",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseManagement_HouseholdRegs_HouseholdId",
                table: "CaseManagement",
                column: "HouseholdId",
                principalTable: "HouseholdRegs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChangeNotes_AspNetUsers_CreatedById",
                table: "ChangeNotes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChildHealthRecords_AspNetUsers_CreatedById",
                table: "ChildHealthRecords",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChildHealthRecords_HealthFacilities_HealthFacilityId",
                table: "ChildHealthRecords",
                column: "HealthFacilityId",
                principalTable: "HealthFacilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChildHealthRecords_Children_ChildId",
                table: "ChildHealthRecords",
                column: "ChildId",
                principalTable: "Children",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChildHealthRecords_Pregnancies_PregnancyId",
                table: "ChildHealthRecords",
                column: "PregnancyId",
                principalTable: "Pregnancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintNotes_AspNetUsers_CreatedById",
                table: "ComplaintNotes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintNotes_Complaints_ComplaintId",
                table: "ComplaintNotes",
                column: "ComplaintId",
                principalTable: "Complaints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_ApprovedById",
                table: "Complaints",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_ClosedById",
                table: "Complaints",
                column: "ClosedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_CreatedById",
                table: "Complaints",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_EscalatedById",
                table: "Complaints",
                column: "EscalatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_ResolvedById",
                table: "Complaints",
                column: "ResolvedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_HealthFacilities_HealthFacilityId",
                table: "Complaints",
                column: "HealthFacilityId",
                principalTable: "HealthFacilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Villages_VillageId",
                table: "Complaints",
                column: "VillageId",
                principalTable: "Villages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Counties_AspNetUsers_CreatedById",
                table: "Counties",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Counties_AspNetUsers_ModifiedById",
                table: "Counties",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Counties_GeoMaster_GeoMasterId",
                table: "Counties",
                column: "GeoMasterId",
                principalTable: "GeoMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HouseholdRegs_HouseholdRegMembers_MotherId",
                table: "HouseholdRegs",
                column: "MotherId",
                principalTable: "HouseholdRegMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Counties_AspNetUsers_CreatedById",
                table: "Counties");

            migrationBuilder.DropForeignKey(
                name: "FK_Counties_AspNetUsers_ModifiedById",
                table: "Counties");

            migrationBuilder.DropForeignKey(
                name: "FK_Divisions_AspNetUsers_CreatedById",
                table: "Divisions");

            migrationBuilder.DropForeignKey(
                name: "FK_Divisions_AspNetUsers_ModifiedById",
                table: "Divisions");

            migrationBuilder.DropForeignKey(
                name: "FK_GeoMaster_AspNetUsers_CreatedById",
                table: "GeoMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_GeoMaster_AspNetUsers_ModifiedById",
                table: "GeoMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_HouseholdRegs_AspNetUsers_ApprovedById",
                table: "HouseholdRegs");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_AspNetUsers_CreatedById",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_AspNetUsers_ModifiedById",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_SubLocations_AspNetUsers_CreatedById",
                table: "SubLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_SubLocations_AspNetUsers_ModifiedById",
                table: "SubLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_HouseholdRegs_HealthFacilities_HealthFacilityId",
                table: "HouseholdRegs");

            migrationBuilder.DropForeignKey(
                name: "FK_Wards_SubCounties_SubCountyId",
                table: "Wards");

            migrationBuilder.DropForeignKey(
                name: "FK_HouseholdRegs_CommunityAreas_CommunityAreaId",
                table: "HouseholdRegs");

            migrationBuilder.DropForeignKey(
                name: "FK_HouseholdRegMembers_HouseholdRegs_HouseholdId",
                table: "HouseholdRegMembers");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditTrail");

            migrationBuilder.DropTable(
                name: "BeneficiaryPaymentPoints");

            migrationBuilder.DropTable(
                name: "CaseManagementSummary");

            migrationBuilder.DropTable(
                name: "ChangeNotes");

            migrationBuilder.DropTable(
                name: "ChildDevelopmentMilestones");

            migrationBuilder.DropTable(
                name: "ChildFeedingInformation");

            migrationBuilder.DropTable(
                name: "ChildHealthRecords");

            migrationBuilder.DropTable(
                name: "CivilRegistrations");

            migrationBuilder.DropTable(
                name: "CommunityValidationSummary");

            migrationBuilder.DropTable(
                name: "ComplaintNotes");

            migrationBuilder.DropTable(
                name: "ComplaintsSummary");

            migrationBuilder.DropTable(
                name: "CutOffs");

            migrationBuilder.DropTable(
                name: "CVListDetails");

            migrationBuilder.DropTable(
                name: "DevelopmentMilestones");

            migrationBuilder.DropTable(
                name: "DrugsAdministered");

            migrationBuilder.DropTable(
                name: "EnrolmentDetails");

            migrationBuilder.DropTable(
                name: "EnrolmentSummary");

            migrationBuilder.DropTable(
                name: "EnumeratorDevices");

            migrationBuilder.DropTable(
                name: "FamilyPlannings");

            migrationBuilder.DropTable(
                name: "FingerPrintVerifications");

            migrationBuilder.DropTable(
                name: "HouseholdRegAssets");

            migrationBuilder.DropTable(
                name: "HouseholdRegCharacteristics");

            migrationBuilder.DropTable(
                name: "HouseholdRegMemberDisabilities");

            migrationBuilder.DropTable(
                name: "HouseholdRegOtherProgrammes");

            migrationBuilder.DropTable(
                name: "InstallationSetup");

            migrationBuilder.DropTable(
                name: "KakamegaVillages");

            migrationBuilder.DropTable(
                name: "MailSettings");

            migrationBuilder.DropTable(
                name: "MotherPreventiveServices");

            migrationBuilder.DropTable(
                name: "MpesaAuthorizationCode");

            migrationBuilder.DropTable(
                name: "MPesaFeedBack");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "PaymentsSummary");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PMT_Household");

            migrationBuilder.DropTable(
                name: "PMT_Rural");

            migrationBuilder.DropTable(
                name: "PMT_Urban");

            migrationBuilder.DropTable(
                name: "PMTCTInterventions");

            migrationBuilder.DropTable(
                name: "PMTCutOffs");

            migrationBuilder.DropTable(
                name: "PMTScores");

            migrationBuilder.DropTable(
                name: "PMTWeights");

            migrationBuilder.DropTable(
                name: "PostNatalMilestones");

            migrationBuilder.DropTable(
                name: "PrePayrollChecksDetails");

            migrationBuilder.DropTable(
                name: "PrePayrollTransactions");

            migrationBuilder.DropTable(
                name: "Registration");

            migrationBuilder.DropTable(
                name: "RegistrationSummary");

            migrationBuilder.DropTable(
                name: "ReportingPeriods");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "ReportTypes");

            migrationBuilder.DropTable(
                name: "RoleProfiles");

            migrationBuilder.DropTable(
                name: "Scoring_Coef");

            migrationBuilder.DropTable(
                name: "SMS");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TempTable");

            migrationBuilder.DropTable(
                name: "TestData");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Changes");

            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "Enumerators");

            migrationBuilder.DropTable(
                name: "CVLists");

            migrationBuilder.DropTable(
                name: "PregnancyData");

            migrationBuilder.DropTable(
                name: "NotesCategories");

            migrationBuilder.DropTable(
                name: "PMTCTServices");

            migrationBuilder.DropTable(
                name: "KeyMilestones");

            migrationBuilder.DropTable(
                name: "PostNatalExaminationDetails");

            migrationBuilder.DropTable(
                name: "PrePayrollActions");

            migrationBuilder.DropTable(
                name: "PayrollExceptions");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PrePayrollChecks");

            migrationBuilder.DropTable(
                name: "ReportCategories");

            migrationBuilder.DropTable(
                name: "SystemTasks");

            migrationBuilder.DropTable(
                name: "ComplaintTypes");

            migrationBuilder.DropTable(
                name: "ComplaintStatus");

            migrationBuilder.DropTable(
                name: "Children");

            migrationBuilder.DropTable(
                name: "PostNatalExaminations");

            migrationBuilder.DropTable(
                name: "PayrollExceptionTypes");

            migrationBuilder.DropTable(
                name: "Beneficiaries");

            migrationBuilder.DropTable(
                name: "Payrolls");

            migrationBuilder.DropTable(
                name: "PaymentStatus");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "Enrolments");

            migrationBuilder.DropTable(
                name: "FundRequests");

            migrationBuilder.DropTable(
                name: "MotherClinicVisits");

            migrationBuilder.DropTable(
                name: "PaymentCycles");

            migrationBuilder.DropTable(
                name: "ApprovalStatus");

            migrationBuilder.DropTable(
                name: "ClinicVisits");

            migrationBuilder.DropTable(
                name: "Pregnancies");

            migrationBuilder.DropTable(
                name: "PaymentPoints");

            migrationBuilder.DropTable(
                name: "CaseManagement");

            migrationBuilder.DropTable(
                name: "CaseManagementStatus");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "HealthFacilities");

            migrationBuilder.DropTable(
                name: "SubCounties");

            migrationBuilder.DropTable(
                name: "CommunityAreas");

            migrationBuilder.DropTable(
                name: "HouseholdRegs");

            migrationBuilder.DropTable(
                name: "HouseholdRegMembers");

            migrationBuilder.DropTable(
                name: "Programmes");

            migrationBuilder.DropTable(
                name: "Reasons");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Villages");

            migrationBuilder.DropTable(
                name: "SystemCodeDetails");

            migrationBuilder.DropTable(
                name: "SubLocations");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "SystemCodes");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "SystemModules");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Constituencies");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "GeoMaster");
        }
    }
}
