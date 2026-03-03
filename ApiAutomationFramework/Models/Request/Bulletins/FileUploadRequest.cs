using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.Bulletins
{
    public class FileUploadRequest
    {
        public int pclmapid { get; set; }
        public int fileaccess { get; set; }
        public string filePath { get; set; } // Local path to the file to upload
    }
}

