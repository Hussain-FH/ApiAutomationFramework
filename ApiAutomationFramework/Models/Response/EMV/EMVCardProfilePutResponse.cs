using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Response.EMV
{
    public class EMVCardProfilePutResponse
    {
        public string userName { get; set; }
        public string name { get; set; }
        public int issuerId { get; set; }
        public string expirationDate { get; set; }
        public string Description { get; set; }
        public int cardprofileid { get; set; }
    }
}
