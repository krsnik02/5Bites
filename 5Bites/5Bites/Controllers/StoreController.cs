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
        [HttpPost]
        public ActionResult Transfer(StoreTransferModel model)
        {
            return RedirectToAction("Transfer", "Store", new { toId = model.ToId, fromId = model.FromId });
        }

        [HttpGet]
        public ActionResult Transfer(int toId, int fromId)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Sell(int id)
        {
            return View();
        }

    }
}
