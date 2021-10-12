using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Studentscreeningsystem.Models;

namespace Studentscreeningsystem.Controllers
{
    public class LoginController : Controller
    {
        private RBAC_Model database = new RBAC_Model();

        // GET: USERs
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(USER user)
        {
            USER usr = database.USERS.FirstOrDefault(u => u.Username == user.Username && u.Passeword == user.Passeword);
            if (usr != null)
            {
                Session["User_Id"] = usr.User_Id;
                Session["Username"] = usr.Username.ToString();
                if (usr.Lastname != null)
                {
                    Session["Lastname"] = usr.Lastname.ToString();
                }
                else
                {
                    Session["Lastname"] = "";
                }
                Session["Firstname"] = usr.Firstname.ToString();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", " رقم الهوية أو الرقم العسكري غير صحيح");

            }
            return View();

        }
        public ActionResult LogOut()
        {
            Session.Remove("User_Id");
            Session.Remove("Username");
            Session.Remove("Lastname");
            Session.Remove("Firstname");
            return RedirectToAction("Login", "Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}