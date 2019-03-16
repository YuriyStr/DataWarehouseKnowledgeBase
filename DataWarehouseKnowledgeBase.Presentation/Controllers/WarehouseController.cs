using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataWarehouseKnowledgeBase.DAL.KbModels;
using DataWarehouseKnowledgeBase.DAL.Repository;
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
        private readonly IKbSerializer<KnowledgeBase> _serializer;

        public WarehouseController(IRepository repository, IHostingEnvironment hostingEnvironment, IKbSerializer<KnowledgeBase> serializer)
        {
            _repository = repository;
            _serializer = serializer;
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
            _serializer.Serialize(new KnowledgeBase
            {
                Parameter = "p",
                Rules = new List<Rule>
                {
                    new Rule
                    {
                        ThenAttributeName = "Kek",
                        ThenAttributeValue = "Cheburek",
                        RequiredAttributes = "PriceCondition",
                        Conditions = new List<Condition>
                        {
                            new ConditionGroup
                            {
                                GroupType = "OR",
                                Conditions = new List<Condition>
                                {
                                    new ConditionNode { Condition = "test1" },
                                    new ConditionNode { Condition = "test2" }
                                }
                            },
                            new ConditionNode
                            {
                                Condition = "test3"
                            }
                        }
                    }
                }
            });
            var knowledgeBase = _serializer.Deserialize();
            ViewBag.Message = knowledgeBase.Rules[0].Conditions[0].InvertResult;
            //ViewBag.Message = KbEvaluator.Test();

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
                if (m.UnitsSold > 0 && m.MoneySold / m.UnitsSold < 100)
                {
                    m.Recommendation = "Необходимо поднять цену на товар";
                }
            });
            return View(models);
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