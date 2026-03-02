using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.BulkPrgUpload
{
    public class BulkPrgCancelUpdateReq
    {
        
            public int uploadedS3FileId { get; set; }
            public bool isProgramCreateFlag { get; set; }
        


    }
}
