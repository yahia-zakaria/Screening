using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Studentscreeningsystem.Controllers
{
    public class UnauthorisedController : Controller
    {
        // GET: Unauthorised
        public ActionResult Index()
        {
            //Session.Abandon();
            return View();
        }
    }
}