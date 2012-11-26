using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class ProductSellModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
    }
}