﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Inventory_.Transfer
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int OldQuantity { get; set; }
    }
}