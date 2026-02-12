using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Request.Categories
{
    public class CategoriesturnonoffRequest
    {
        public int pclid { get; set; }
        public int categoryId { get; set; }
        public bool active { get; set; }
    }
}
