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
        /**
         * GET /Home/Index
         * Employee Home Page
         * If there is no logged in user, redirect to /Store/Inventory
         */
        [HttpGet]
        public ActionResult Index()
        {
            if (Session.Contents["EmployeeId"] == null)
                return RedirectToAction("Inventory", "Store");
            return View();
        }
    }
}
