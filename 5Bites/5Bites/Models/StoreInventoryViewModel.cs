using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class StoreInventoryViewModel
    {
        public List<StoreInventoryModel> Stores { get; set; }

        public StoreInventoryViewModel()
        {
            Stores = new List<StoreInventoryModel>();
        }
    }
}