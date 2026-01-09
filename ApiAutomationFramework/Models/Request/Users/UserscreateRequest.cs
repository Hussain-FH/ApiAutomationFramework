using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.Users
{
    public class UserscreateRequest
    {
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public List<int> pclIds { get; set; }
        public List<int> roleIds { get; set; }
        public int StatusId { get; set; }

    }
}
