using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Inventory.Purchase
{
    public class LocationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductModel> Inventory { get; set; }

        public LocationModel()
        {
            Inventory = new List<ProductModel>();
        }
    }
}