using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.bcssconfigurations
{
    public class bcssconfUpdateRequest
    {
        public string name { get; set; }
        public string description { get; set; }
        public string profileName { get; set; }
        public int cvvDateFormatId { get; set; }
        public int cvV2DateFormatId { get; set; }
        public int cvvKeyPairId { get; set; }
        public int cvV2KeyPairId { get; set; }
        public int serviceCodeId { get; set; }
        public int useServiceCodeCVVId { get; set; }
        public int useServiceCodeCVV2Id { get; set; }
        public object pvkiCode { get; set; }
        public object cvkType { get; set; }
        public string pclid { get; set; }
        public int id { get; set; }

    }
}
