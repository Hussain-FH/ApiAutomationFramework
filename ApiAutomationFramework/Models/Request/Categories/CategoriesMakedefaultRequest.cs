using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.Categories
{
    public class CategoriesMakedefaultRequest
    {
        public int PclId { get; set; }
        public int CategoryId { get; set; }
        public int CategoryTypeId { get; set; }
    }
}
