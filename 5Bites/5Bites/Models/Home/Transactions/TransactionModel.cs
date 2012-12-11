using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Home.Transactions
{
    public class TransactionModel
    {
        public String StoreName { get; set; }
        public String ProductName { get; set; }
        public String EmployeeName { get; set; }
        public int Quantity { get; set; }
        public DateTime Timestamp { get; set; }
    }
}