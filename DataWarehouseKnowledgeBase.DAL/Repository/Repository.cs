using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataWarehouseKnowledgeBase.DAL.DbModels;
using DataWarehouseKnowledgeBase.DAL.DwModels;
using DataWarehouseKnowledgeBase.DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using Sales = DataWarehouseKnowledgeBase.DAL.DbModels.Sales;

namespace DataWarehouseKnowledgeBase.DAL.Repository
{
    public class Repository : IRepository
    {
        private readonly DatabaseContext _dbContext;
        private readonly DwContext _dwContext;

        public Repository(DatabaseContext dbContext, DwContext dwContext)
        {
            _dbContext = dbContext;
            _dwContext = dwContext;
        }

        public bool AddNewSale(string storeCode, string productCode, decimal price)
        {
            return AddNewSale(storeCode, productCode, price, DateTime.Now);
        }

        public bool AddNewSale(string storeCode, string productCode, decimal price, DateTime saleDate)
        {
            int? storeId = _dbContext.Store.SingleOrDefault(s => s.StoreNumber == storeCode)?.StoreId;
            if (storeId == null)
                return false;
            int? productId = _dbContext.Product.SingleOrDefault(p => p.ProductCode == productCode)?.ProductId;
            if (productId == null)
                return false;
            var sale = new DbModels.Sales
            {
                StoreStoreId = storeId.Value,
                ProductProductId = productId.Value,
                Price = price,
                SaleDate = saleDate,
                InWarehouse = false
            };
            _dbContext.Sales.Add(sale);
            _dbContext.SaveChanges();
            return true;
        }

        public void UpdateWarehouse()
        {
            AddNewStores();
            AddNewProducts();
            AddNewSales();
            _dbContext.SaveChanges();
            _dwContext.SaveChanges();
        }

        public IEnumerable<WarehouseViewModel> FetchFromWarehouse(string productCode, string storeCode, DateTime? time)
        {
            IQueryable<DwModels.Sales> query = _dwContext.Sales.Include(s => s.ProductProduct).Include(s => s.StoreStore)
                .Include(s => s.TimeTime);
            if (productCode != null)
                query = query.Where(s => s.ProductProduct.ProductCode == productCode);
            if (storeCode != null)
                query = query.Where(s => s.StoreStore.StoreNumber == storeCode);
            if (time != null)
                query = query.Where(s => s.TimeTime.Year == time.Value.Year && s.TimeTime.MonthNo == time.Value.Month
                                                                            && s.TimeTime.Day == time.Value.Day);
            var result = query.GroupBy(s => new { s.ProductProduct.ProductId, s.StoreStore.StoreId, s.TimeTime.TimeId })
                .Select(g => new WarehouseViewModel
                {
                    UnitsSold = g.Sum(s => s.UnitsSold ?? 0),
                    MoneySold = g.Sum(s => s.MoneySold ?? 0),
                    ProductCode = g.Max(s => s.ProductProduct.ProductCode),
                    ProductName = g.Max(s => s.ProductProduct.ProductName),
                    BrandName = g.Max(s => s.ProductProduct.BrandName),
                    ProductCategory = g.Max(s => s.ProductProduct.ProductCategory),
                    StoreCode = g.Max(s => s.StoreStore.StoreNumber),
                    City = g.Max(s => s.StoreStore.City),
                    Country = g.Max(s => s.StoreStore.Country),
                    Year = g.Max(s => s.TimeTime.Year),
                    Month = g.Max(s => s.TimeTime.MonthNo),
                    Day = g.Max(s => s.TimeTime.Day),
                    WeekDay = g.Max(s => s.TimeTime.WeekDay)
                });
            return result;
        }

        public AverageMoneyAndUnits CalculateAverageMoneyAndUnits(string productCode)
        {
            if (productCode == null) return null;
            IQueryable<DwModels.Sales> query = _dwContext.Sales.Include(s => s.ProductProduct)
                .Where(s => s.ProductProduct.ProductCode == productCode);
            return query.GroupBy(s => s.ProductProduct.ProductId)
                .Select(g => new AverageMoneyAndUnits
                {
                    AverageUnits = g.Average(s => s.UnitsSold ?? 0),
                    AverageMoney = g.Average(s => s.MoneySold ?? 0)
                })
                .FirstOrDefault();
        }

        private void AddNewStores()
        {
            var newStores = _dbContext.Store.Include(s => s.LocationLocation).ThenInclude(l => l.CountryCountry)
                .Where(s => !s.InWarehouse);
            var newStoresW = newStores.Select(s => new DwModels.Store
                {
                    StoreId = s.StoreId,
                    StoreNumber = s.StoreNumber,
                    City = s.LocationLocation.CityName,
                    Country = s.LocationLocation.CountryCountry.CountryName
                });
            _dwContext.Store.AddRange(newStoresW);
            foreach (var store in newStores)
            {
                store.InWarehouse = true;
                _dbContext.Entry(store).State = EntityState.Modified;
            }
        }

        private void AddNewProducts()
        {
            var newProducts = _dbContext.Product.Include(p => p.BrandBrand).Include(p => p.CategoryCategory)
                .Where(p => !p.InWarehouse);
            var newProductsW = newProducts.Select(p => new DwModels.Product
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    BrandName = p.BrandBrand.BrandName,
                    ProductCategory = p.CategoryCategory.CategoryName
                });
            _dwContext.Product.AddRange(newProductsW);
            foreach (var product in newProducts)
            {
                product.InWarehouse = true;
                _dbContext.Entry(product).State = EntityState.Modified;
            }
        }

        private void AddNewSales()
        {
            var newSales = _dbContext.Sales.Where(p => !p.InWarehouse).ToList();
            var newSalesW = newSales.GroupBy(s => new {s.SaleDate, s.StoreStoreId, s.ProductProductId})
                .Select(g => new
                {
                    MoneySold = g.Sum(s => s.Price),
                    UnitsSold = g.Count(),
                    StoreStoreId = g.Key.StoreStoreId,
                    ProductProductId = g.Key.ProductProductId,
                    Day = g.Key.SaleDate.Day,
                    Month = g.Key.SaleDate.Month,
                    Year = g.Key.SaleDate.Year,
                    SaleDate = g.Key.SaleDate
                })
                .OrderBy(s => s.Year).ThenBy(s => s.Month).ThenBy(s => s.Day).ThenBy(s => s.StoreStoreId).ThenBy(s => s.ProductProductId);
            foreach (var sale in newSalesW)
            {
                int? dayId = _dwContext.Times.FirstOrDefault(t => t.Year == sale.Year && t.MonthNo == sale.Month && t.Day == sale.Day)?.TimeId;
                bool isNew = dayId == null;
                DwModels.Sales existingSale = null;
                if (isNew)
                {
                    dayId = AddDay(sale.Day, sale.Month, sale.Year, sale.SaleDate.ToString("dddd", CultureInfo.InvariantCulture), sale.SaleDate.ToString("MMMM", CultureInfo.InvariantCulture));
                }
                else
                {
                    existingSale = _dwContext.Sales.FirstOrDefault(s =>
                        s.TimeTimeId == dayId && s.ProductProductId == sale.ProductProductId &&
                        s.StoreStoreId == sale.StoreStoreId);
                }
                AddSales(sale.UnitsSold, sale.MoneySold, dayId.Value, sale.ProductProductId, sale.StoreStoreId, existingSale);
            }
            foreach (var sale in newSales)
            {
                sale.InWarehouse = true;
                _dbContext.Entry(sale).State = EntityState.Modified;
            }
        }

        private int AddDay(int day, int month, int year, string weekDay, string monthName)
        {
            var time = new DwModels.Times
            {
                Day = day,
                MonthNo = month,
                Year = year,
                WeekDay = weekDay,
                MonthName = monthName
            };
            _dwContext.Times.Add(time);
            _dwContext.SaveChanges();
            return time.TimeId;
        }

        private void AddSales(decimal unitsSold, decimal moneySold, int timeId, int productId,
            int storeId, DwModels.Sales existingSale)
        {
            if (existingSale == null)
            {
                var entity = new DwModels.Sales
                {
                    UnitsSold = unitsSold,
                    MoneySold = moneySold,
                    TimeTimeId = timeId,
                    ProductProductId = productId,
                    StoreStoreId = storeId
                };
                _dwContext.Sales.Add(entity);
            }
            else
            {
                existingSale.UnitsSold = unitsSold;
                existingSale.MoneySold = moneySold;
                _dwContext.Entry(existingSale).State = EntityState.Modified;
            }
            _dwContext.SaveChanges();
        }
    }
}
