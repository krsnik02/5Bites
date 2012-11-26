using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class HomeIndexViewModel
    {
        public StoreTransferModel PostTransfer { get; set; }

        public string EmployeeName { get; set; }
        public bool IsAdmin { get; set; }
        public List<LocationModel> Locations { get; set; }

        public HomeIndexViewModel()
        {
            Locations = new List<LocationModel>();
        }
    }
}