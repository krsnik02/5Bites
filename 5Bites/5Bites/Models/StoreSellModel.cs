using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class StoreSellModel
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public List<ProductSellModel> Inventory { get; set; }

        public StoreSellModel()
        {
            Inventory = new List<ProductSellModel>();
        }
    }
}