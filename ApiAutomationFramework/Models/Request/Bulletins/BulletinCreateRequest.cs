using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.Bulletins
{
    public class CSPBulletinsCreateRequest
    {
        public string bulletinName { get; set; }
        public string description { get; set; }
        public DateTime publishDate { get; set; }
        public List<int> pclIds { get; set; }
        public bool isArchived { get; set; }
        public int? bulletinId { get; set; }
        public int fileUploadedId { get; set; }
    }
}
