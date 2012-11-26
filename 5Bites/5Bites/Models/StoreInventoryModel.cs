using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class StoreInventoryModel
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public List<ProductInventoryModel> Inventory { get; set; }

        public StoreInventoryModel()
        {
            Inventory = new List<ProductInventoryModel>();
        }
    }
}