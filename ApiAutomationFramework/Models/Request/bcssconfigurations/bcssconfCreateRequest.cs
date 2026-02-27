using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.bcssconfigurations
{
    public class bcssconfCreateRequest
    {
        public string name { get; set; }
        public string description { get; set; }
        public string profileName { get; set; }
        public int cvvDateFormatId { get; set; }
        public int cvV2DateFormatId { get; set; }
        public int cvvKeyPairId { get; set; }
        public int cvV2KeyPairId { get; set; }
        public int serviceCodeId { get; set; }
        public string pclid { get; set; }



    }
}
