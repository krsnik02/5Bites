using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _5Bites.Controllers
{
    public class StoreController : Controller
    {
        //
        // GET: /Store/

        public ActionResult Transfer(int FromId, int ToId)
        {
            return View();
        }

    }
}
