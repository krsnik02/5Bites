using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Employee_.Permissions
{
    public class ViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
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