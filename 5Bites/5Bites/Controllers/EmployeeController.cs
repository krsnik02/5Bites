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
            SHA256 sha256 = new SHA256Managed();
            byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(m.Password));

            using (var db = new DBContext())
            {
                var employee = db.Employees.SingleOrDefault(
                    e => e.Username == m.Username && e.Password == hashed);
                Session.Contents["EmployeeId"] = employee.Id;
                Session.Contents["EmployeeName"] = employee.Username;
                Session.Contents["EmployeeAdmin"] = employee.IsAdmin;
            }

            if (Session.Contents["EmployeeId"] == null)
                return RedirectToAction("Login", "Employee");
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Account()
        {
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

            using (var db = new DBContext())
            {
                var employees = from e in db.Employees
                                where e.Id != EmployeeId
                                select e;
                var stores = db.Stores;
                var locations = db.Locations;

                foreach (var employee in employees)
                {
                    m.Employees.Add(new Models.Employee_.Manage.EmployeeModel
                    {
                        Id = employee.Id,
                        Username = employee.Username
                    });
                }

                foreach (var store in stores)
                {
                    m.Hire.Stores.Add(new Models.Employee_.Hire.StoreModel
                    {
                        Id = store.Id,
                        Name = store.Location.Name
                    });
                }

                foreach (var location in locations)
                {
                    m.Hire.Locations.Add(new Models.Employee_.Hire.LocationModel
                    {
                        Id = location.Id,
                        Name = location.Name
                    });
                }
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult Hire(Models.Employee_.Hire.ViewModel m)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(m.Password));

            using (var db = new DBContext())
            {
                var e = new Employee();
                e.Username = m.Username;
                e.Password = hashed;
                e.IsAdmin = m.IsAdmin;

                foreach (var s in m.Stores)
                {
                    if (s.HasAccess)
                    {
                        var es = new EmployeeStore();
                        es.Employee = e;
                        es.StoreId = s.Id;
                        e.EmployeeStores.Add(es);
                    }
                }

                foreach (var l in m.Locations)
                {
                    if (l.HasAccess)
                    {
                        var el = new EmployeeLocation();
                        el.Employee = e;
                        el.LocationId = l.Id;
                        e.EmployeeLocations.Add(el);
                    }
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

            using (var db = new DBContext())
            {
                var e = db.Employees.Single(x => x.Id == id);
                m.Id = e.Id;
                m.Username = e.Username;
                m.IsAdmin = e.IsAdmin;

                foreach(var s in db.Stores)
                {
                    m.Stores.Add(new Models.Employee_.Permissions.StoreModel
                    {
                        Id = s.Id,
                        Name = s.Location.Name,
                        HasAccess = e.EmployeeStores.Count(x => x.Store == s) != 0
                    });
                }

                foreach (var l in db.Locations)
                {
                    m.Locations.Add(new Models.Employee_.Permissions.LocationModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        HasAccess = e.EmployeeLocations.Count(x => x.Location == l) != 0
                    });
                }
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult Permissions(_5Bites.Models.Employee_.Permissions.ViewModel m)
        {
            using (var db = new DBContext())
            {
                var e = db.Employees.Single(x => x.Id == m.Id);
                e.IsAdmin = m.IsAdmin;

                e.EmployeeStores.Clear();
                db.EmployeeStores.Where(el => el.EmployeeId == e.Id)
                    .ToList().ForEach(el => db.EmployeeStores.Remove(el));
                m.Stores.Where(l => l.HasAccess)
                    .ToList().ForEach(l => db.EmployeeStores.Add(
                        new EmployeeStore
                        {
                            Employee = e,
                            StoreId = l.Id
                        }));

                e.EmployeeLocations.Clear();
                db.EmployeeLocations.Where(el => el.EmployeeId == e.Id)
                    .ToList().ForEach(el => db.EmployeeLocations.Remove(el));
                m.Locations.Where(l => l.HasAccess)
                    .ToList().ForEach(l => db.EmployeeLocations.Add(
                        new EmployeeLocation
                        {
                            Employee = e,
                            LocationId = l.Id
                        }));

                db.SaveChanges();
            }

            return RedirectToAction("Manage", "Employee");
        }
    }
}
