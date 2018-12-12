using System;
using System.Collections.Generic;

namespace DataWarehouseKnowledgeBase.DAL.DbModels
{
    public partial class Country
    {
        public Country()
        {
            Locations = new HashSet<Locations>();
        }

        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public ICollection<Locations> Locations { get; set; }
    }
}
