using System;
using System.Collections.Generic;

namespace DataWarehouseKnowledgeBase.DAL.DbModels
{
    public partial class Sales
    {
        public int SaleId { get; set; }
        public decimal Price { get; set; }
        public DateTime SaleDate { get; set; }
        public int StoreStoreId { get; set; }
        public int ProductProductId { get; set; }
        public bool InWarehouse { get; set; }

        public Product ProductProduct { get; set; }
        public Store StoreStore { get; set; }
    }
}
