using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Response.Categories
{
    public class CategoriesCreateResponse
    {
        public string Name { get; set; }
        public int ParentCategoryId { get; set; }
        public int PclId { get; set; }
    }
}
