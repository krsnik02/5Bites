using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Home.Transactions
{
    public class ViewModel
    {
        public List<TransactionModel> Transactions { get; set; }

        public ViewModel()
        {
            Transactions = new List<TransactionModel>();
        }
    }
}