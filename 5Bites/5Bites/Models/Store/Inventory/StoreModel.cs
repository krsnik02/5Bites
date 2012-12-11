using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Store_.Inventory
{
    public class StoreModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductModel> Inventory { get; set; }

        public StoreModel()
        {
            Inventory = new List<ProductModel>();
        }
    }
}