using System;
using System.Collections.Generic;

namespace DataWarehouseKnowledgeBase.DAL.DbModels
{
    public partial class Locations
    {
        public Locations()
        {
            Store = new HashSet<Store>();
        }

        public int LocationId { get; set; }
        public string CityName { get; set; }
        public int CountryCountryId { get; set; }

        public Country CountryCountry { get; set; }
        public ICollection<Store> Store { get; set; }
    }
}
