using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.IssuingbanksArchive
{
    public class ArchiveUpdateRequest
    {
        public int id { get; set; }
        public bool IsArchive { get; set; }
    }
}
