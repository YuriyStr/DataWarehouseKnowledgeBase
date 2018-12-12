using System;
using System.Collections.Generic;

namespace DataWarehouseKnowledgeBase.DAL.DbModels
{
    public partial class Store
    {
        public Store()
        {
            Sales = new HashSet<Sales>();
        }

        public int StoreId { get; set; }
        public string StoreNumber { get; set; }
        public int LocationLocationId { get; set; }
        public bool InWarehouse { get; set; }

        public Locations LocationLocation { get; set; }
        public ICollection<Sales> Sales { get; set; }
    }
}
