using _5Bites.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _5Bites.Controllers
{
    public class InventoryController : Controller
    {
        public ActionResult Transfer()
        {
            int EmployeeId = (int)Session.Contents["EmployeeId"];
            var m = new Models.Inventory_.Transfer.ViewModel();

            using (var db = new dbEntities())
            {
                var ps = db.Products;
                var ls = db.EmployeeLocations.Where(el => el.EmployeeId == EmployeeId).Select(el => el.Location);

                foreach (var l in ls)
                {
                    var lm = new Models.Inventory_.Transfer.LocationModel();
                    lm.Id = l.Id;
                    lm.Name = l.Name;
                    foreach (var p in ps)
                    {
                        var pm = new Models.Inventory_.Transfer.ProductModel();
                        pm.Id = p.Id;
                        pm.Name = p.Name;
                        pm.Quantity = p.Inventories.SingleOrDefault(i => i.Location == l).Quantity;
                        lm.Inventory.Add(pm);
                    }
                    m.Locations.Add(lm);
                }
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult Transfer(Models.Inventory_.Transfer.ViewModel m)
        {
            using (var db = new dbEntities())
            {
                foreach (var lm in m.Locations)
                {
                    foreach (var pm in lm.Inventory)
                    {
                        db.Inventories.Single(i => i.LocationId == lm.Id && i.ProductId == pm.Id).Quantity = pm.Quantity;
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("Transfer", "Inventory");
        }

        [HttpGet]
        public ActionResult Purchase()
        {
            int EmployeeId = (int)Session.Contents["EmployeeId"];
            var m = new Models.Inventory_.Purchase.ViewModel();

            using (var db = new dbEntities())
            {
                var ps = db.Products;
                var ls = db.EmployeeLocations.Where(el => el.EmployeeId == EmployeeId).Select(el => el.Location);

                foreach (var l in ls)
                {
                    var lm = new Models.Inventory_.Purchase.LocationModel();
                    lm.Id = l.Id;
                    lm.Name = l.Name;
                    foreach (var p in ps)
                    {
                        var pm = new Models.Inventory_.Purchase.ProductModel();
                        pm.Id = p.Id;
                        pm.Name = p.Name;
                        pm.Price = p.WholesalePrice;
                        pm.OldQuantity = p.Inventories.SingleOrDefault(i => i.Location == l).Quantity;
                        lm.Inventory.Add(pm);
                    }
                    m.Locations.Add(lm);
                }
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult Purchase(Models.Inventory_.Purchase.ViewModel m)
        {
            if (ModelState.IsValid)
            {
                using (var db = new dbEntities())
                {
                    foreach (var lm in m.Locations)
                    {
                        foreach (var pm in lm.Inventory)
                        {
                            db.Inventories.Single(i => i.LocationId == lm.Id && i.ProductId == pm.Id).Quantity += pm.Quantity;
                        }
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Purchase", "Inventory");
            }
            return View();
        }

    }
}
