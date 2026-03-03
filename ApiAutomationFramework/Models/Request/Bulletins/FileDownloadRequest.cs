using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.Bulletins
{
    public class FileDownloadRequest
    {
        public int fileformat { get; set; }
        public int entityid { get; set; }
    }
}
