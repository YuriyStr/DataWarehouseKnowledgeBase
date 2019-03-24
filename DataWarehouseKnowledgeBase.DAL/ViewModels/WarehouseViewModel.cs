namespace DataWarehouseKnowledgeBase.DAL.ViewModels
{
    public class WarehouseViewModel
    {
        public decimal UnitsSold { get; set; }
        public decimal MoneySold { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string ProductCategory { get; set; }
        public string StoreCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string WeekDay { get; set; }
        public double OverallCondition { get; set; }
    }
}