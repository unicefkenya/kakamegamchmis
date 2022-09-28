namespace MCHAPP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Ward
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? SubCountyId { get; set; }

        public virtual SubCounty SubCounty { get; set; }
    }
}