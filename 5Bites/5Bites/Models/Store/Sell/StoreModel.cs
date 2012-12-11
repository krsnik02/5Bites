using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Store_.Sell
{
    public class StoreModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        public decimal Bank { get; set; }

        public List<ProductModel> Inventory { get; set; }

        public StoreModel()
        {
            Inventory = new List<ProductModel>();
        }
    }
}