using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.ClientSettiClientprofiles
{
    public class ClientprofilesUpdateRequest
    {
        public string value { get; set; }
        public string userName { get; set; }
        public int pclId { get; set; }
        public int keyId { get; set; }
    }
}
