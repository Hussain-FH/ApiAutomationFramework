using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.EMV
{
    public class EMV_Module_Create_Request
    {
   
        public string name { get; set; }
        public string description { get; set; }
        public string travelerLabel { get; set; }
        public string cmiProgram { get; set; }
        public int groupId { get; set; }
        public string userName { get; set; }
    }
}
