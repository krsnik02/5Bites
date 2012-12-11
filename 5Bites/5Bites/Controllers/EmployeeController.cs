using _5Bites.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace _5Bites.Controllers
{
    public class EmployeeController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.Employee_.Login.ViewModel m)
        {
            if (ModelState.IsValid)
            {
                SHA256 sha256 = new SHA256Managed();
                byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(m.Password));

                using (var db = new dbEntities())
                {
                    var employee = db.Employees.SingleOrDefault(
                        e => e.Username == m.Username && e.Password == hashed);
                    if (employee == null)
                        return View();

                    Session.Contents["EmployeeId"] = employee.Id;
                    Session.Contents["EmployeeName"] = employee.Username;
                    Session.Contents["EmployeeAdmin"] = employee.IsAdmin;
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Account()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Account(Models.Employee_.Account.ViewModel m)
        {
            if (ModelState.IsValid)
            {
                SHA256 sha256 = new SHA256Managed();
                byte[] oldhashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(m.OldPassword));
                byte[] newhashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(m.NewPassword));

                int EmployeeId = (int)Session.Contents["EmployeeId"];
                using (var db = new dbEntities())
                {
                    var e = db.Employees.Single(e_ => e_.Id == EmployeeId);
                    if (e.Password == oldhashed) e.Password = newhashed;
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Contents["EmployeeId"] = null;
            Session.Contents["EmployeeName"] = null;
            Session.Contents["EmployeeAdmin"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Manage()
        {
            // Enforce admin priviledges
            if (!((bool?)Session.Contents["EmployeeAdmin"] ?? false))
                return RedirectToAction("Index", "Home");

            int EmployeeId = (int)Session.Contents["EmployeeId"];
            var m = new Models.Employee_.Manage.ViewModel();

            using (var db = new dbEntities())
            {
                foreach (var e in db.Employees.Where(e => e.Id != EmployeeId))
                {
                    var em = new Models.Employee_.Manage.EmployeeModel();
                    em.Id = e.Id;
                    em.Username = e.Username;
                    m.Employees.Add(em);
                }

                foreach (var s in db.Stores)
                {
                    var sm = new Models.Employee_.Hire.StoreModel();
                    sm.Id = s.Id;
                    sm.Name = s.Location.Name;
                    m.Hire.Stores.Add(sm);
                }

                foreach (var l in db.Locations)
                {
                    var lm = new Models.Employee_.Hire.LocationModel();
                    lm.Id = l.Id;
                    lm.Name = l.Name;
                    m.Hire.Locations.Add(lm);
                }
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult Hire(Models.Employee_.Hire.ViewModel m)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(m.Password));

            using (var db = new dbEntities())
            {
                var e = new Employee();
                e.Username = m.Username;
                e.Password = hashed;
                e.IsAdmin = m.IsAdmin;

                foreach (var s in m.Stores.Where(s => s.HasAccess))
                {
                    var es = new EmployeeStore();
                    es.Employee = e;
                    es.StoreId = s.Id;
                    e.EmployeeStores.Add(es);
                }

                foreach (var l in m.Locations.Where(l => l.HasAccess))
                {
                    var el = new EmployeeLocation();
                    el.Employee = e;
                    el.LocationId = l.Id;
                    e.EmployeeLocations.Add(el);
                }

                db.Employees.Add(e);
                db.SaveChanges();
            }

            return RedirectToAction("Manage", "Employee");
        }

        [HttpGet]
        public ActionResult Permissions(int id)
        {
            if (!((bool?)Session.Contents["EmployeeAdmin"] ?? false))
                return RedirectToAction("Index", "Home");

            var m = new _5Bites.Models.Employee_.Permissions.ViewModel();

            using (var db = new dbEntities())
            {
                var e = db.Employees.Single(e_ => e_.Id == id);
                m.Id = e.Id;
                m.Username = e.Username;
                m.IsAdmin = e.IsAdmin;

                foreach(var s in db.Stores)
                {
                    var sm = new Models.Employee_.Permissions.StoreModel();
                    sm.Id = s.Id;
                    sm.Name = s.Location.Name;
                    sm.HasAccess = e.EmployeeStores.Count(es => es.Store == s) != 0;
                    m.Stores.Add(sm);
                }

                foreach (var l in db.Locations)
                {
                    var lm = new Models.Employee_.Permissions.LocationModel();
                    lm.Id = l.Id;
                    lm.Name = l.Name;
                    lm.HasAccess = e.EmployeeLocations.Count(el => el.Location == l) != 0;
                    m.Locations.Add(lm);
                }
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult Permissions(_5Bites.Models.Employee_.Permissions.ViewModel m)
        {
            using (var db = new dbEntities())
            {
                var e = db.Employees.Single(e_ => e_.Id == m.Id);
                e.IsAdmin = m.IsAdmin;

                e.EmployeeStores.Clear();
                e.EmployeeLocations.Clear();
                db.EmployeeStores.Where(el => el.EmployeeId == e.Id)
                    .ToList().ForEach(el => db.EmployeeStores.Remove(el));
                db.EmployeeLocations.Where(el => el.EmployeeId == e.Id)
                    .ToList().ForEach(el => db.EmployeeLocations.Remove(el));

                foreach (var s in m.Stores.Where(s => s.HasAccess))
                {
                    var es = new EmployeeStore();
                    es.Employee = e;
                    es.StoreId = s.Id;
                    db.EmployeeStores.Add(es);
                }

                foreach (var l in m.Locations.Where(l => l.HasAccess))
                {
                    var el = new EmployeeLocation();
                    el.Employee = e;
                    el.LocationId = l.Id;
                    db.EmployeeLocations.Add(el);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Manage", "Employee");
        }
    }
}
