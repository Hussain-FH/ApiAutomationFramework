using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.Categories
{
    public class CategoriesRenameRequest
    {
        public string name { get; set; }
        public int pclId { get; set; }
        public int categoryId { get; set; }


    }
}
