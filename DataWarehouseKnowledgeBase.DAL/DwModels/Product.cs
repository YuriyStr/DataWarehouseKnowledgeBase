using System;
using System.Collections.Generic;

namespace DataWarehouseKnowledgeBase.DAL.DwModels
{
    public partial class Product
    {
        public Product()
        {
            Sales = new HashSet<Sales>();
        }

        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string ProductCategory { get; set; }
        public int ProductId { get; set; }

        public ICollection<Sales> Sales { get; set; }
    }
}
