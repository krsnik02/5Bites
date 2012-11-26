using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class StoreSellViewModel
    {
        public List<StoreSellModel> Stores { get; set; }

        public StoreSellViewModel()
        {
            Stores = new List<StoreSellModel>();
        }
    }
}