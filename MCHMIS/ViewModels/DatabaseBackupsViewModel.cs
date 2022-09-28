using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MCHMIS.ViewModels
{
    public class DatabaseBackupsList
    {
        public string Name { get; set; }
        public string PhysicalPath { get; set; }
        public DateTimeOffset LastModified { get; set; }

        [DisplayName("Size")]
        public long Length { get; set; }
    }
}