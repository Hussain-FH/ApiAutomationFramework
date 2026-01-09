using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Response.EMV
{

    public class EMVCardProfilesCreateResponse
    {
        public string userName { get; set; }
        public string name { get; set; }
        public int issuerId { get; set; }
        public string expirationDate { get; set; }
        public string discription { get; set; }
    }

}
