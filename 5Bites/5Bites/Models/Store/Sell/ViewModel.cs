using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Store_.Sell
{
    public class ViewModel
    {
        public List<StoreModel> Stores { get; set; }

        public ViewModel()
        {
            Stores = new List<StoreModel>();
        }
    }
}