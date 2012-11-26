using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class StoreSellViewModel
    {
        public string StoreName { get; set; }
        public List<ProductSaleModel> Inventory { get; set; }

        public StoreSellViewModel()
        {
            Inventory = new List<ProductSaleModel>();
        }
    }
}