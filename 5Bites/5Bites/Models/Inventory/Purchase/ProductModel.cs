using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Inventory_.Purchase
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Range(0.0, Double.PositiveInfinity)]
        public int Quantity { get; set; }

        public int OldQuantity { get; set; }
    }
}