using System;
using System.Collections.Generic;

namespace DataWarehouseKnowledgeBase.DAL.DwModels
{
    public partial class Times
    {
        public Times()
        {
            Sales = new HashSet<Sales>();
        }

        public int Day { get; set; }
        public string WeekDay { get; set; }
        public int MonthNo { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }
        public int TimeId { get; set; }

        public ICollection<Sales> Sales { get; set; }
    }
}
