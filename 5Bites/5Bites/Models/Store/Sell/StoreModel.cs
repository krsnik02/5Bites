using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Store_.Sell
{
    public class StoreModel
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public List<ProductModel> Inventory { get; set; }

        public StoreModel()
        {
            Inventory = new List<ProductModel>();
        }
    }
}