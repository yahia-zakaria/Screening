using Studentscreeningsystem.ViewsModel;
using Studentscreeningsystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Studentscreeningsystem.Controllers
{
  
    public class GraduateWishesVMController : Controller
    {
        private RBAC_Model db = new RBAC_Model();
        // GET: GraduateWishesVM
        public ActionResult Index(int? IdGraduate)
        {


            if (IdGraduate == null)
            {
                IdGraduate = Int32.Parse(Session["User_Id"].ToString());
                var graduateWishes = db.GraduateWishes.Include(g => g.USER).Include(g => g.Sector).Where(g => g.User_Id == IdGraduate);
                var graduate = db.USERS.Find(IdGraduate);
                if (graduate != null)
                {
                    ViewBag.NameGraduate = graduate.Lastname + " " + graduate.Firstname;
                    ViewBag.Passeword = graduate.Passeword;
                    ViewBag.userName = graduate.Username;
                    }
                return View(graduateWishes.OrderBy(g => g.Rank).ToList());
            }
            else
            {
                var graduateWishes = db.GraduateWishes.Include(g => g.USER).Include(g => g.Sector).Where(g => g.User_Id == IdGraduate);
                if (graduateWishes != null)
                {
                    return View(graduateWishes.OrderBy(g => g.Rank).ToList());
                }
                else
                {
                    TempData["ErreurWishes"] = "لم يقم الخريج  بترتيب رغباته ";
                    return RedirectToAction("ListGraduate", "Admin");
                }

            }

        }

        // GET: GraduateWishes/Create
        public ActionResult SortableWishes()
        {
            if (Session["User_Id"] != null)
            {
                int IdGraduate = (int)Session["User_Id"];
                var _user = db.USERS.Find(IdGraduate);
                if (_user != null)
                {
                    if (_user.Title == "خريج")
                    {
                        List<int> results = db.Database.SqlQuery<int>(string.Format("SELECT User_Id FROM GraduateWishes WHERE User_Id = '{0}'", IdGraduate)).ToList();
                        bool _WishesExistsInTable = (results.Count > 0);

                        if (_WishesExistsInTable)
                        {
                            TempData["DidGraduateWishes"] = "لقد قمت بعملية ترتيب الرغبات سابقاً ";
                            return RedirectToAction("Index", "GraduateWishesVM");
                        }
                        else
                        {
                            IList<Sector> ListSector = db.Sector.ToList();
                            ViewBag.ListSector = ListSector;

                            ViewBag.IdGraduate = IdGraduate;

                            return View();
                        }
                    }
                    else
                    {
                        TempData["message"] = "تقتصر عملية ترتيب الرغبات  فقط على الخريجين";
                        return RedirectToAction("Login", "Login");
                    }

                }
                else
                {
                    TempData["message"] = "هذا الخريج غير موجود";
                    return RedirectToAction("Login", "Login");
                }
            }
            else
            {
                TempData["message"] = "session vide";
                return RedirectToAction("Login", "Login");
            }
            




        }

        // POST: GraduateWishes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SortableWishes(GraduateWishesVM graduateWishesVM)
        {
            if (ModelState.IsValid)
            {
                var _user = db.USERS.Find(graduateWishesVM.IdGraduate);
                if (_user != null)
                {
                    if (_user.Title == "خريج")
                    {
                        List<int> results = db.Database.SqlQuery<int>(string.Format("SELECT User_Id FROM GraduateWishes WHERE User_Id = '{0}'", graduateWishesVM.IdGraduate)).ToList();
                        bool _WishesExistsInTable = (results.Count > 0);

                        if (_WishesExistsInTable)
                        {
                            TempData["DidWishes"] = "لقد قمت بعملية ترتيب الرغبات سابقاً";
                            return RedirectToAction("Index", "GraduateWishesVM");
                        }
                        else
                        {
                            var i = 1;
                            foreach (var item in graduateWishesVM.IdSectorList)
                            {
                                var wishes = new GraduateWishes
                                {
                                    User_Id = graduateWishesVM.IdGraduate,
                                    IdSector = item.IdSector,
                                    Rank = i
                                };
                                db.GraduateWishes.Add(wishes);
                                db.SaveChanges();
                                i++;

                            }
                            TempData["successdidWishes"] = "   لقد قمت بعملية ترتيب الرغبات بنجاح";


                            return RedirectToAction("Index", "GraduateWishesVM");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Login", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Login");
                }
            }

            ViewBag.IdGraduate = 1;
            IList<Sector> ListSector = db.Sector.ToList();
            ViewBag.ListSector = ListSector;
            return View(graduateWishesVM);
        }
        public ActionResult EditSortableWishes()
        {

            int IdGraduate = Int32.Parse(Session["User_Id"].ToString());
            var _user = db.USERS.Find(IdGraduate);
            if (_user != null)
            {
                if (_user.Title == "خريج")
                {
                    List<int> results = db.Database.SqlQuery<int>(string.Format("SELECT User_Id FROM GraduateWishes WHERE User_Id = '{0}'", IdGraduate)).ToList();
                    bool _WishesExistsInTable = (results.Count > 0);

                    if (_WishesExistsInTable)
                    {

                        var ListSector = db.GraduateWishes.Include(g => g.Sector).Where(g => g.User_Id == IdGraduate).OrderBy(g => g.Rank).ToList();

                        ViewBag.ListSector = ListSector;
                        ViewBag.IdGraduate = IdGraduate;

                        return View();
                    }
                    else
                    {
                        TempData["PleasedoWishes"] = "الرجاء القيام بعملية ترتيب الرغبات ";
                        return RedirectToAction("SortableWishes", "GraduateWishesVM");
                    }
                }
                else
                {
                    TempData["message"] = "تقتصر عملية ترتيب الرغبات  فقط على الخريجين";
                    return RedirectToAction("Login", "Login");
                }

            }
            else
            {

                return RedirectToAction("Login", "Login");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSortableWishes(GraduateWishesVM graduateWishesVM)
        {
            if (ModelState.IsValid)
            {
                var _user = db.USERS.Find(graduateWishesVM.IdGraduate);
                if (_user != null)
                {
                    if (_user.Title == "خريج")
                    {
                        List<int> results = db.Database.SqlQuery<int>(string.Format("SELECT User_Id FROM GraduateWishes WHERE User_Id = '{0}'", graduateWishesVM.IdGraduate)).ToList();
                        bool _WishesExistsInTable = (results.Count > 0);

                        if (_WishesExistsInTable)
                        {

                            var i = 1;
                            foreach (var item in graduateWishesVM.IdSectorList)
                            {
                                var wishes = db.GraduateWishes.Find(item.Id);
                                Console.WriteLine(item.Id);
                                wishes.IdSector = item.IdSector;
                                Console.WriteLine(wishes.IdSector);
                                wishes.User_Id = graduateWishesVM.IdGraduate;
                                Console.WriteLine(wishes.User_Id);
                                wishes.Rank = i;
                                Console.WriteLine(wishes.Rank);
                                db.Entry(wishes).State = EntityState.Modified;
                                db.SaveChanges();
                                i++;
                            }
                            TempData["successdidWishes"] = "لقد قمت بعملية ترتيب الرغبات بنجاح";
                            return RedirectToAction("Index", "GraduateWishesVM");
                        }
                        else
                        {
                            TempData["PleasedoWishes"] = "الرجاء القيام بعملية ترتيب الرغبات ";
                            return RedirectToAction("SortableWishes", "GraduateWishesVM");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Login", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Login");
                }
            }

            ViewBag.IdGraduate = 1;
            IList<Sector> ListSector = db.Sector.ToList();
            ViewBag.ListSector = ListSector;
            return View(graduateWishesVM);
        }
    }
}