using _5Bites.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _5Bites.Controllers
{
    public class StoreController : Controller
    {
        [HttpGet]
        public ActionResult Inventory()
        {
            var m = new _5Bites.Models.Store_.Inventory.ViewModel();
            using (var db = new dbEntities())
            {
                var ss = db.Stores.Include("Location");
                foreach (var s in ss)
                {
                    var sm = new Models.Store_.Inventory.StoreModel();
                    sm.Id = s.Id;
                    sm.Name = s.Location.Name;
                    foreach (var i in s.Location.Inventories)
                    {
                        var pm = new Models.Store_.Inventory.ProductModel();
                        pm.Description = i.Product.Name;
                        pm.Price = i.Product.RetailPrice;
                        pm.Quantity = i.Quantity;
                        sm.Inventory.Add(pm);
                    }
                    m.Stores.Add(sm);
                }
            }

            return View(m);
        }

        [HttpGet]
        public ActionResult Sell()
        {
            int EmployeeId = (int)Session.Contents["EmployeeId"];
            var m = new _5Bites.Models.Store_.Sell.ViewModel();

            using (var db = new dbEntities())
            {
                var ss = db.EmployeeStores.Where(es => es.EmployeeId == EmployeeId).Select(es => es.Store);
                foreach (var s in ss)
                {
                    var sm = new Models.Store_.Sell.StoreModel();
                    sm.Id = s.Id;
                    sm.Bank = s.Bank;
                    sm.Name = s.Location.Name;
                    foreach (var i in s.Location.Inventories)
                    {
                        var pm = new Models.Store_.Sell.ProductModel();
                        pm.Id = i.Product.Id;
                        pm.Description = i.Product.Name;
                        pm.Price = i.Product.RetailPrice;
                        pm.Quantity = i.Quantity;
                        sm.Inventory.Add(pm);
                    }
                    m.Stores.Add(sm);
                }
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult Sell(_5Bites.Models.Store_.Sell.StoreModel m)
        {
            int EmployeeId = (int)Session.Contents["EmployeeId"];

            using (var db = new dbEntities())
            {
                var s = db.Stores.Single(s_ => s_.Id == m.Id);
                foreach (var pm in m.Inventory.Where(pm => pm.Quantity != 0))
                {
                    var t = new Transaction();
                    t.ProductId = pm.Id;
                    t.StoreId = m.Id;
                    t.EmployeeId = EmployeeId;
                    t.Quantity = pm.Quantity;
                    t.Timestamp = DateTime.Now;
                    db.Transactions.Add(t);

                    s.Location.Inventories.Single(i => i.ProductId == pm.Id).Quantity -= pm.Quantity;
                }
                s.Bank += m.Inventory.Sum(pm => pm.Quantity * pm.Price);
                db.SaveChanges();
            }

            return RedirectToAction("Sell", "Store");
        }

    }
}
