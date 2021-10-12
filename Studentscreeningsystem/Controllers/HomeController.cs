using Studentscreeningsystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;

namespace Studentscreeningsystem.Controllers
{
    public class HomeController : Controller
    {
        private RBAC_Model db = new RBAC_Model();
        //[RBAC]
        public ActionResult Index()
        {
            var IdGraduate = Int32.Parse(Session["User_Id"].ToString());
           var Isgraduate = db.USERS.Where(g => g.Title == "خريج" && g.User_Id == IdGraduate).ToList().Count();
            if (Isgraduate == 1)
            {
                return RedirectToAction("Index", "GraduateWishesVM");
            }
            else
            {
                //عدد الخريجين
                var NbG = db.USERS.Where(g => g.Title == "خريج").ToList().Count();

                //عدد المستخدمين
                var Nbusers = db.USERS.ToList().Count();

                //عدد الخريجين الذين تم إدخال درجاتهم
                var NbGD = from t1 in db.SpecificationGraduate
                           join t2 in db.USERS
                           on t1.User_Id equals t2.User_Id
                           where t2.Title == "خريج"
                           select (new { t2.Username, t2.Firstname, t2.Lastname, t2.Passeword });

                //عدد الخريجين الذين لم يتم إدخال درجاتهم
                var NbGND = (from t1 in db.USERS
                             join t2 in db.SpecificationGraduate on t1.User_Id equals t2.User_Id into j
                             from t2 in j.DefaultIfEmpty()
                             where t2 == null && t1.Title == "خريج"
                             select new { t1.Username, t1.Firstname, t1.Lastname, t1.Passeword }).ToList().Count();

                //عدد الخريجين الذين لم يقوموا بإدخال رغباتهم
                var NbGNW = (from t1 in db.USERS
                             join t2 in db.GraduateWishes on t1.User_Id equals t2.User_Id into j
                             from t2 in j.DefaultIfEmpty()
                             where t2 == null && t1.Title == "خريج"
                             select new { t1.Username, t1.Firstname, t1.Lastname, t1.Passeword }).ToList().Count();

                //عدد الأسلحة
                var NbSector = db.Sector.ToList().Count();

                //عدد الخريجين الذين أتموا إدخال رغباتهم
                var NBNG = (from t1 in db.USERS
                            join t2 in db.GraduateWishes
                            on t1.User_Id equals t2.User_Id
                            where t1.Title == "خريج"
                            select (new { t1.User_Id, t1.Username, t1.Firstname, t1.Lastname, t1.Passeword })).Distinct().ToList().Count();


                //عدد الخريجين المستبعدين من عملية التوزيع
                var NBGInsort = db.USERS.Where(g => g.Title == "خريج" && g.Insortable == true).ToList().Count();



                //Apply ViewBag

                ViewBag.NbG = NbG;
                ViewBag.Nbusers = Nbusers;
                ViewBag.NbGD = NbGD.ToList().Count();
                ViewBag.NbSector = NbSector;
                ViewBag.NBNG = NBNG;
                ViewBag.NBGInsort = NBGInsort;
                ViewBag.NbGND = NbGND;
                ViewBag.NbGNW = NbGNW;

                return View();
            }
        }
        
    }
}