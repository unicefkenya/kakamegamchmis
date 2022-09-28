using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class PMT_Rural
    {
        public int Id { get; set; }
        public float? region_Nairobi { get; set; }

        public float? region_Central { get; set; }

        public float? region_Mombasa { get; set; }

        public float? region_Coastal { get; set; }

        public float? region_Upper_Eastern { get; set; }

        public float? region_Mid_Eastern { get; set; }

        public float? region_Lower_Eastern { get; set; }

        public float? region_North_Eastern { get; set; }

        public float? region_Nyanza { get; set; }

        public float? region_North_Rift { get; set; }

        public float? region_Central_Rift { get; set; }

        public float? region_South_Rift { get; set; }

        public float? region_Western { get; set; }

        public float? cont_population { get; set; }

        public float? cont_deathsrate { get; set; }

        public float? cont_birthsrate { get; set; }

        public float? cont_Mean_precip { get; set; }

        public float? cont_Mean_elev { get; set; }

        public float? cont_work_subloc { get; set; }

        public float? cont_work_agrs { get; set; }

        public float? cont_selfemp_subloc { get; set; }

        public string SubLocVar { get; set; }

        public string Subloc { get; set; }
    }

    public class PMT_Urban
    {
        public int Id { get; set; }
        public float? region_Nairobi { get; set; }

        public float? region_Central { get; set; }

        public float? region_Mombasa { get; set; }

        public float? region_Coastal { get; set; }

        public float? region_Upper_Eastern { get; set; }

        public float? region_Mid_Eastern { get; set; }

        public float? region_Lower_Eastern { get; set; }

        public float? region_North_Eastern { get; set; }

        public float? region_Nyanza { get; set; }

        public float? region_North_Rift { get; set; }

        public float? region_Central_Rift { get; set; }

        public float? region_South_Rift { get; set; }

        public float? region_Western { get; set; }

        public float? cont_population { get; set; }

        public float? cont_deathsrate { get; set; }

        public float? cont_birthsrate { get; set; }

        public float? cont_Mean_precip { get; set; }

        public float? cont_Mean_elev { get; set; }

        public float? cont_work_subloc { get; set; }

        public float? cont_work_agrs { get; set; }

        public float? cont_selfemp_subloc { get; set; }

        public string SubLocVar { get; set; }

        public string Subloc { get; set; }
    }

    public class Scoring_Coef
    {
        [Key]
        public string ItemId { get; set; }

        public float? Coefficient { get; set; }

        public float? Mean { get; set; }

        public float? StdDev { get; set; }

        public float? Coefficient1 { get; set; }

        public float? Mean1 { get; set; }

        public float? StdDev1 { get; set; }

        public string Region_Urban { get; set; }

        public float? Coefficient2 { get; set; }

        public float? Mean2 { get; set; }

        public float? StdDev2 { get; set; }

        public string Region_Rural { get; set; }

        public string F13 { get; set; }

        public string F14 { get; set; }

        public string F15 { get; set; }
    }

    public class PMT_Household
    {
        [Key]
        public string HouseholdId { get; set; }

        public decimal? PMT_Dwelling { get; set; }

        public decimal? PMT_HHMemberSize { get; set; }

        public decimal? PMT_HHRoomsPerPersons { get; set; }

        public decimal? PMT_Wall { get; set; }

        public decimal? PMT_ROOF { get; set; }

        public decimal? PMT_FLOOR { get; set; }

        public decimal? PMT_WATER { get; set; }

        public decimal? PMT_TOILET { get; set; }

        public decimal? PMT_COOK { get; set; }

        public decimal? PMT_LIGHT { get; set; }

        public decimal? PMT_TV { get; set; }

        public decimal? PMT_MOTORCYCLE { get; set; }

        public decimal? PMT_CAR { get; set; }

        public decimal? PMT_REFRIGERATOR { get; set; }

        public decimal? PMT_TUKTUK { get; set; }

        public decimal? PMT_EXOTIC { get; set; }

        public decimal? PMT_ZEBU { get; set; }

        public decimal? PMT_SHEEP { get; set; }

        public decimal? PMT_GOATS { get; set; }

        public decimal? PMT_CAMELS { get; set; }

        public decimal? PMT_DONKEYS { get; set; }

        public decimal? PMT_MALEHEAD { get; set; }

        public decimal? PMT_SPOUSE { get; set; }

        public decimal? PMT_MONOGAMOUS { get; set; }

        public decimal? PMT_MALEMEMBERS { get; set; }

        public decimal? PMT_HEADAGE { get; set; }

        public decimal? PMT_MEANHOUSEHOLDAGE { get; set; }

        public decimal? PMT_SPOUSEAGE { get; set; }

        public decimal? PMT_DEPENDANCY { get; set; }

        public decimal? PMT_CHILDRENUNDER6 { get; set; }

        public decimal? PMT_ORPHAN { get; set; }

        public decimal? PMT_UNDER12SCHOOL { get; set; }

        public decimal? PMT_HEADEDUCATION { get; set; }

        public decimal? PMT_SPOUSEEDUCATION { get; set; }

        public decimal? PMT_MAXHOUSEHOLDEDUCATION { get; set; }

        public decimal? PMT_DISABLED15TO65 { get; set; }

        public decimal? PMT_DEATHINHOUSEHOLD { get; set; }

        public decimal? PMT_BIRTHINHOUSEHOLD { get; set; }

        public decimal? PMT_HEADWORK { get; set; }

        public decimal? PMT_SPOUSEWORK { get; set; }

        public decimal? PMT_WORKING15TO65 { get; set; }

        public decimal? PMT_WORKING6TO15 { get; set; }

        public decimal? PMT_WAGEWORKERSSUBLOCATION { get; set; }

        public decimal? PMT_AGRIWORKERSSUBLOCATION { get; set; }

        public decimal? PMT_SELFEMPLOYEDSUBLOCATION { get; set; }

        public decimal? PMT_POPULATIONSUBLOCATION { get; set; }

        public decimal? PMT_DEATHRATESUBLOCATION { get; set; }

        public decimal? PMT_BIRTHRATESUBLOCATION { get; set; }

        public decimal? PMT_PRECIPITATIONSUBLOCATION { get; set; }

        public decimal? PMT_ELEVATIONSUBLOCATION { get; set; }

        public decimal? PMT_SCORE { get; set; }

        public string Household_Group { get; set; }

        public string Household_Rank { get; set; }

        public bool? PWSD { get; set; }

        public bool? OVC { get; set; }

        public bool? OP { get; set; }

        public bool? HSNP { get; set; }

        public bool? MultipleVulnerability { get; set; }

        public string FinalProgramme { get; set; }

        public decimal? LCS { get; set; }

        public decimal? PMT_Coastal { get; set; }

        public decimal? PMT_UpperEastern { get; set; }

        public decimal? PMT_LowerEastern { get; set; }

        public decimal? PMT_NorthEastern { get; set; }

        public decimal? PMT_Nyanza { get; set; }

        public decimal? PMT_NorthRift { get; set; }

        public decimal? PMT_CentralRift { get; set; }

        public decimal? PMT_Western { get; set; }

        public string PWSD_Member { get; set; }

        public string OVC_Member { get; set; }

        public string OP_Member { get; set; }
    }
}