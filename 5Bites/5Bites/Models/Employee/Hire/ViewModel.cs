using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Employee_.Hire
{
    public class ViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public List<StoreModel> Stores { get; set; }
        public List<LocationModel> Locations { get; set; }

        public ViewModel()
        {
            Stores = new List<StoreModel>();
            Locations = new List<LocationModel>();
        }
    }
}