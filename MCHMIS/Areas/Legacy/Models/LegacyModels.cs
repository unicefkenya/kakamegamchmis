using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MCHMIS.Areas.Legacy.Models
{
    public class Dirty : ApprovalFields
    {
        public int Id { get; set; }

        public string MotherId { get; set; }

        public string db { get; set; }

        [DisplayName("Reason Value")]
        public string Field { get; set; }

        public string Reason { get; set; }
        public string MotherUniqueId { get; set; }

        [ForeignKey("MotherUniqueId")]
        public Mother Mother { get; set; }

        public int StatusId { get; set; }
        public Status Status { get; set; }

        [DisplayName("Approval Status")]
        public int? ApprovalStatusId { get; set; }

        public Status ApprovalStatus { get; set; }

        public string Notes { get; set; }
        public DateTime? DateEdited { get; set; }
        public string EditedById { get; set; }
        public string ApprovalNotes { get; set; }
        public string SupportingDocument { get; set; }
    }

    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Mother
    {
        [Key]
        public string UniqueId { get; set; }

        public string Motherid { get; set; }

        public string M_Names { get; set; }

        public string M_IDNo { get; set; }

        public string M_DOB { get; set; }

        public string M_County { get; set; }

        public string M_Ward { get; set; }

        public string M_Phoneno { get; set; }

        public string M_Photo { get; set; }

        public string M_Address { get; set; }

        public string M_Occupation { get; set; }

        public string M_Income { get; set; }

        public string M_Flag { get; set; }

        public string F_Names { get; set; }

        public string F_IDNo { get; set; }

        public string F_DOB { get; set; }

        public string F_County { get; set; }

        public string F_Ward { get; set; }

        public string F_Phoneno { get; set; }

        public string F_Photo { get; set; }

        public string F_Address { get; set; }

        public string F_Occupation { get; set; }

        public string F_Income { get; set; }

        public string F_Flag { get; set; }

        public string G_Names { get; set; }

        public string G_IDNo { get; set; }

        public string G_DOB { get; set; }

        public string G_County { get; set; }

        public string G_Ward { get; set; }

        public string G_Phoneno { get; set; }

        public string G_Photo { get; set; }

        public string G_Address { get; set; }

        public string G_Occupation { get; set; }

        public string G_Income { get; set; }

        public string G_Relationship { get; set; }

        public string Other_income { get; set; }

        public string facilityid { get; set; }
        [ForeignKey("facilityid")] public Facility Facility { get; set; }
        public string p_flag { get; set; }

        public string CareOf { get; set; }

        public string unqualified_reason { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public string date { get; set; }

        public string db { get; set; }

        public int? DirtyId { get; set; }

        [ForeignKey("DirtyId")]
        public Dirty Dirty { get; set; }
    }

    public class Payments
    {
        public string mpesaid { get; set; }

        public string Names { get; set; }

        public string IDno { get; set; }

        public string phoneno { get; set; }

        public string amount { get; set; }

        public string mpesa_flag { get; set; }

        public string facility { get; set; }

        public string date { get; set; }
    }

    public class Facility
    {
        public string facilityid { get; set; }

        public string names { get; set; }

        public string county { get; set; }

        public string ward { get; set; }

        public string constituency { get; set; }

        public string m_limit { get; set; }

        public string village { get; set; }

        public string db { get; set; }

        public int? newfacilityid { get; set; }
    }

    public class Reason
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}