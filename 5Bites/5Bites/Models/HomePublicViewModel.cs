using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class HomePublicViewModel
    {
        public List<StoreModel> Stores { get; set; }

        public HomePublicViewModel()
        {
            Stores = new List<StoreModel>();
        }
    }
}