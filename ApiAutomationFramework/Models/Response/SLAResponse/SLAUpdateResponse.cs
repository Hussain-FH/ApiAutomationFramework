using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Response.SLAResponse
{
    public class SLAUpdateResponse
    {
        public int id { get; set; }
        public int cardProgramId { get; set; }
        public bool isSpecialProject { get; set; }
        public bool isRepeatedEveryYear { get; set; }
        public int sladay { get; set; }
        public int specialProjectMinimumShipmentCardCount { get; set; }
        public int specialProjectSladay { get; set; }
        public DateTime effectiveDate { get; set; }
        public DateTime endDate { get; set; }
    }
}
