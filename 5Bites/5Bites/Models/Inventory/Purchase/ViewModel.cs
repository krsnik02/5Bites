using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Inventory.Purchase
{
    public class ViewModel
    {
        public List<LocationModel> Locations { get; set; }

        public ViewModel()
        {
            Locations = new List<LocationModel>();
        }
    }
}