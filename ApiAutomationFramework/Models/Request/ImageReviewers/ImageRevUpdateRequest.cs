using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.ImageReviewers
{
     public class ImageRevUpdateRequest
    {
        public int consumerProductId { get; set; }
        public int statusCode { get; set; }
        public string statusCodeName { get; set; }
        public int reasonCode { get; set; }
        public string reasonComments { get; set; }
    }
}
