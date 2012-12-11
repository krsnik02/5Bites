using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Store_.Sell
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        [Required]
        [Range(0.0, Double.PositiveInfinity)]
        [DynamicMaximum("Quantity")]
        public int QuantitySold { get; set; }
    }
}