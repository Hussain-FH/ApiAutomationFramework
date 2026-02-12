using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.ClientSettingsHotStamps
{
    public class CSHotStampsCreateR
    {
        public int pclId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string userName { get; set; }

    }
}
