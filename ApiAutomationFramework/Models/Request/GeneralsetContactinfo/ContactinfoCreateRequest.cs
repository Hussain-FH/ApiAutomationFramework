using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.GeneralsetContactinfo
{
    public  class ContactinfoCreateRequest
    {

        public class AddressesList
        {
            public string street1 { get; set; }
            public string street2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }
            public int countryId { get; set; }
        }

            public string name { get; set; }
            public string title { get; set; }
            public string email { get; set; }
            public string officeNo { get; set; }
            public string mobileNo { get; set; }
            public string otherNo { get; set; }
            public string note { get; set; }
            public List<AddressesList> addressesList { get; set; }
            public int pclId { get; set; }
        








    }
}
