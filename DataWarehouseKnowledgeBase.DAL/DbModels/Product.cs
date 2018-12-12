using System;
using System.Collections.Generic;

namespace DataWarehouseKnowledgeBase.DAL.DbModels
{
    public partial class Product
    {
        public Product()
        {
            Sales = new HashSet<Sales>();
        }

        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int? BrandBrandId { get; set; }
        public int? CategoryCategoryId { get; set; }
        public bool InWarehouse { get; set; }

        public Brand BrandBrand { get; set; }
        public Category CategoryCategory { get; set; }
        public ICollection<Sales> Sales { get; set; }
    }
}
