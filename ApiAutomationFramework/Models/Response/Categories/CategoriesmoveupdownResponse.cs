using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework.Models.Response.Categories
{
    public class CategoriesmoveupdownResponse
    {
        public int pclid { get; set; }
        public bool isUp { get; set; }
        public int categoryId { get; set; }
        public int parentCategoryId { get; set; }

    }
}
