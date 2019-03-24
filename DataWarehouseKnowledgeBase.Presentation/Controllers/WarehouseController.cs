using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataWarehouseKnowledgeBase.DAL.KbModels;
using DataWarehouseKnowledgeBase.DAL.Repository;
using DataWarehouseKnowledgeBase.DAL.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DataWarehouseKnowledgeBase.Presentation.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly IRepository _repository;
        private readonly string _updateFile;
        private DateTime _lastUpdate;
        private int _interval;
        private readonly IKbEvaluator _evaluator;

        public WarehouseController(IRepository repository, IHostingEnvironment hostingEnvironment, IKbEvaluator evaluator)
        {
            _repository = repository;
            _evaluator = evaluator;
            _updateFile = hostingEnvironment.ContentRootPath + "\\update.txt";
            SetTimes();
            CheckForUpdate();
        }

        [HttpGet]
        public IActionResult DbWork()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DbWork(string productCode, string storeCode, decimal? price)
        {
            if (!string.IsNullOrEmpty(productCode) && !string.IsNullOrEmpty(storeCode) && price.HasValue && price.Value > 0
                && _repository.AddNewSale(storeCode, productCode, price.Value))
                ViewBag.Message = "Информация успешно добавлена.";
            else
                ViewBag.Message = "Ошибка. Информация не добавлена в базу.";
            return View();
        }

        [HttpGet]
        public ActionResult DwSettings()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DwSettings(int? interval, string action)
        {
            if (interval.HasValue)
            {
                _interval = interval.Value;
                if (action == "update")
                {
                    UpdateDW(DateTime.Now);
                    ViewBag.Message = "Хранилище обновлено. Настройки сохранены.";
                }
                else
                {
                    WriteTimes();
                    ViewBag.Message = "Настройки сохранены.";
                }
            }
            else
            {
                ViewBag.Message = "Неправильное значение интервала.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult Analysis(string productCode, string storeCode, string dateString, int? page)
        {
            int pageSize = 10;
            page = page ?? 1;
            if (String.IsNullOrEmpty(productCode))
                productCode = null;
            if (String.IsNullOrEmpty(storeCode))
                storeCode = null;
            DateTime? time = null;
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var timeBuffer))
            {
                time = timeBuffer;
            }
            var resultList = _repository.FetchFromWarehouse(productCode, storeCode, time);
            ViewData["ProductCode"] = productCode;
            ViewData["StoreCode"] = storeCode;
            ViewData["DateString"] = dateString;
            ViewData["Page"] = page;
            var models = resultList.Skip(pageSize * (page.Value - 1)).Take(pageSize).ToList();
            models.ForEach(m =>
            {
                m.OverallCondition = GetOverallCondition(m);
            });
            return View(models);
        }

        private double GetOverallCondition(WarehouseViewModel model)
        {
            var averageUnitsAndMoney = _repository.CalculateAverageMoneyAndUnits(model.ProductCode);
            decimal averagePrice = 0, unitsMargin = 0, moneyMargin = 0;
            if (averageUnitsAndMoney.AverageUnits != 0)
            {
                averagePrice = averageUnitsAndMoney.AverageMoney / averageUnitsAndMoney.AverageUnits;
                unitsMargin = averageUnitsAndMoney.AverageUnits / 3;
                moneyMargin = averagePrice / 3;
            }
            var kbParameter = new
            {
                LowerSaleBound = averageUnitsAndMoney.AverageUnits - unitsMargin,
                UpperSaleBound = averageUnitsAndMoney.AverageUnits + unitsMargin,
                LowerPriceBound = averagePrice - moneyMargin,
                UnitsSold = model.UnitsSold,
                MoneySold = model.MoneySold
            };
            return double.Parse(_evaluator.GetAttribute("OverallCondition", kbParameter));
        }

        private void SetTimes()
        {
            string[] textLines = System.IO.File.ReadAllLines(_updateFile);
            _lastUpdate = DateTime.ParseExact(textLines[0], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            _interval = Int32.Parse(textLines[1]);
        }

        private void WriteTimes()
        {
            System.IO.File.WriteAllLines(_updateFile,
                new[] { _lastUpdate.ToString("yyyy-MM-dd HH:mm"), _interval.ToString() });
        }

        private void CheckForUpdate()
        {
            var nextUpdate = _lastUpdate.AddMinutes(_interval);
            var now = DateTime.Now;
            if (now >= nextUpdate)
            {
                UpdateDW(now);
            }
        }

        private void UpdateDW(DateTime now)
        {
            _repository.UpdateWarehouse();
            _lastUpdate = now;
            WriteTimes();
        }
    }
}