using System;
using System.Collections.Generic;

namespace DataWarehouseKnowledgeBase.DAL.DwModels
{
    public partial class Store
    {
        public Store()
        {
            Sales = new HashSet<Sales>();
        }

        public string StoreNumber { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int StoreId { get; set; }

        public ICollection<Sales> Sales { get; set; }
    }
}
