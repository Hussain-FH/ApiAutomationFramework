using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Response.Categories
{
    public class CategoriesturnonoffResponse
    {
        public int pclid { get; set; }
        public int categoryId { get; set; }
        public bool active { get; set; }
    }
}
