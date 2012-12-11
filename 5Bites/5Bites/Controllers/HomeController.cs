using _5Bites.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _5Bites.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (Session.Contents["EmployeeId"] == null)
                return RedirectToAction("Inventory", "Store");
            return View();
        }

        [HttpGet]
        public ActionResult Transactions()
        {
            var m = new Models.Home.Transactions.ViewModel();
            using (var db = new dbEntities())
            {
                var ts = db.Transactions;
                foreach (var t in ts)
                {
                    var tm = new Models.Home.Transactions.TransactionModel();
                    tm.EmployeeName = t.Employee.Username;
                    tm.StoreName = t.Store.Location.Name;
                    tm.ProductName = t.Product.Name;
                    tm.Quantity = t.Quantity;
                    tm.Timestamp = t.Timestamp;
                    m.Transactions.Add(tm);
                }
            }
            return View(m);
        }
    }
}
