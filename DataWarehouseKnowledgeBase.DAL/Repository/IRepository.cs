using System;
using System.Collections.Generic;
using DataWarehouseKnowledgeBase.DAL.ViewModels;

namespace DataWarehouseKnowledgeBase.DAL.Repository
{
    public interface IRepository
    {
        bool AddNewSale(string storeCode, string productCode, decimal price);

        bool AddNewSale(string storeCode, string productCode, decimal price, DateTime saleDate);

        void UpdateWarehouse();

        IEnumerable<WarehouseViewModel> FetchFromWarehouse(string productCode, string storeCode, DateTime? time);

        AverageMoneyAndUnits CalculateAverageMoneyAndUnits(string productCode);
    }
}
