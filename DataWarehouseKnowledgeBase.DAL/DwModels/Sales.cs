using System;
using System.Collections.Generic;

namespace DataWarehouseKnowledgeBase.DAL.DwModels
{
    public partial class Sales
    {
        public decimal? MoneySold { get; set; }
        public decimal? UnitsSold { get; set; }
        public int ProductProductId { get; set; }
        public int StoreStoreId { get; set; }
        public int TimeTimeId { get; set; }

        public Product ProductProduct { get; set; }
        public Store StoreStore { get; set; }
        public Times TimeTime { get; set; }
    }
}
