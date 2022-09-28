using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MCHMIS.Models
{
    public class County : CreateModifyFields
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "County Code", Description = "County code defined by Bureau of Statistics")]
        [StringLength(20)]
        public string Code { get; set; }

        [JsonIgnore]
        public ICollection<Constituency> Constituencies { get; set; }

        [JsonIgnore]
        public GeoMaster GeoMaster { get; set; }

        [Required]
        [Display(Name = "GeoMaster", Description = "The name of the Geo Master")]
        public int GeoMasterId { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "County", Description = "The name of the county")]
        public string Name { get; set; }
    }

    public class SubCounty
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }
        public int CountyId { get; set; }
        public County County { get; set; }
    }

    public class Ward
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        public SubCounty SubCounty { get; set; }

        [DisplayName("Constituency")]
        public int ConstituencyId { get; set; }

        public Constituency Constituency { get; set; }
    }

    public class Constituency
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }
        public int CountyId { get; set; }
        public County County { get; set; }
    }

    public class District
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "District Id")]
        public int Id { get; set; }

        public GeoMaster GeoMaster { get; set; }

        [Display(Name = "Geo Master Id")]
        [Required]
        public int GeoMasterId { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "District Name")]
        public string Name { get; set; }

        public int ConstituencyId { get; set; }
        public Constituency Constituency { get; set; }
    }

    public class Division : CreateModifyFields
    {
        [Display(Name = "Division")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Display(Name = "Division Name")]
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Location> Locations { get; set; }

        public int DistrictId { get; set; }
        public District District { get; set; }
    }

    public class Location : CreateModifyFields
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Location")]
        public int Id { get; set; }

        [Display(Name = "Location Name")]
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<SubLocation> SubLocations { get; set; }

        public Division Division { get; set; }

        [Display(Name = "Division")]
        public int DivisionId { get; set; }
    }

    public class SubLocation : CreateModifyFields
    {
        [Display(Name = "Sub Location")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(30)]
        [Required]
        [Display(Name = "Sub Location Name")]
        public string Name { get; set; }

        [JsonIgnore]
        public Location Location { get; set; }

        [Display(Name = "Location")]
        public int LocationId { get; set; }

        public string RuralUrban { get; set; }

        [Display(Name = "Ward")]
        public int? WardId { get; set; }
    }

    public class Village
    {
        [Display(Name = "Village ID")]
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Display(Name = "Village Unit")]
        [StringLength(30), Required]
        public string Name { get; set; }

        public string Code { get; set; }

        [JsonIgnore]
        public SubLocation SubLocation { get; set; }

        [Display(Name = "Sub Location")]
        public int? SubLocationId { get; set; }

        [Display(Name = "Ward")]
        public int WardId { get; set; }

        public Ward Ward { get; set; }
    }

    public class CommunityArea
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public Village Village { get; set; }
        [Display(Name = "Community Area")]
        [StringLength(30), Required]
        public string Name { get; set; }
        public string Code { get; set; }
    }

    
    public class GeoMaster : CreateModifyFields
    {
        public ICollection<County> Counties { get; set; }

        [Display(Name = "Description", Description = "Short description about the Geo Master"), Required]
        [StringLength(100)]
        public string Description { get; set; }

        public ICollection<District> Districts { get; set; }

        [Display(Name = "Geo Master", Description = "Geo Master ID")]
        public int Id { get; set; }

        [Display(Name = "Default GeoMaster", Description = "Flag to set the Default GeoMaster in the System")]
        public bool IsDefault { get; set; }

        [Display(Name = "Geo Master", Description = "The name of the Geo Master")]
        [StringLength(20), Required]
        // [Index(IsUnique = true, IsClustered = false)]
        public string Name { get; set; }
    }

    public class KakamegaVillage
    {
        public int Id { get; set; }
        public string Ward { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}