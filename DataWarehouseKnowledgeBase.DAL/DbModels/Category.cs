using System;
using System.Collections.Generic;

namespace DataWarehouseKnowledgeBase.DAL.DbModels
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public ICollection<Product> Product { get; set; }
    }
}
