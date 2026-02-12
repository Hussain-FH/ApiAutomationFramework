using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.Categories
{
    public class CategoriesCreateRequest
    {
        public string Name { get; set; }
        public int ParentCategoryId { get; set; }
        public int PclId { get; set; }
    }
}
