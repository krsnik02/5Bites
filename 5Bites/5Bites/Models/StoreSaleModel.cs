using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class StoreSaleModel
    {
        public int Id;
        public Dictionary<int, ProductSaleModel> Inventory;
    }
}