using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.Approval
{
    public class ApprovalCreateRequest
    {

        public int binid { get; set; }
        public int issuebank { get; set; }
        public object producttype { get; set; }
        public object platform { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public object assid { get; set; }
        public object piiorrpp { get; set; }
        public object quantity { get; set; }
        public object foctemplateid { get; set; }
        public object boctemplateid { get; set; }
        public object submisiondate { get; set; }
        public string pclid { get; set; }
        public bool customCardProgram { get; set; }
        public bool isembossed { get; set; }
        public object deftemplateid { get; set; }
        public string mccid { get; set; }
        public string inComment { get; set; }
        public bool isSavedComment { get; set; }
    }
}
