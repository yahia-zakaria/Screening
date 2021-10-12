using Studentscreeningsystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Data.Entity.Validation;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using Studentscreeningsystem.ViewsModel;

namespace Studentscreeningsystem.Controllers
{
    [RBAC]
    public class AdminController : Controller
    {
        private RBAC_Model db = new RBAC_Model();

        #region USERS

        public ActionResult save_user_excel(HttpPostedFileBase file)
        {
            var message = "";
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                file = files[0];
                string fname;
                if (Request.Browser.Browser.ToUpper() == "IE" ||
                    Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] {'\\'});
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                }

                var Pathfile = Path.Combine(Server.MapPath("~/ExcelUsers/"), fname);
                var save = false;
                var i = 0;
                while (save)
                {
                    if (System.IO.File.Exists(Pathfile))
                    {
                        string filename = Path.GetFileNameWithoutExtension(Pathfile);

                        filename = filename + "-" + i++;
                        string extention = Path.GetExtension(Pathfile);
                        filename = filename + extention;
                        Pathfile = Path.Combine(Server.MapPath("~/ExcelUsers/"), filename);
                    }
                    else
                    {
                        save = true;
                    }
                }

                file.SaveAs(Pathfile);

                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + Pathfile +
                                               "';Extended Properties='Excel 12.0 Xml;IMEX=1'";
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                //Sheet Name
                excelConnection.Open();
                string tableName = excelConnection.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                excelConnection.Close();
                //End

                OleDbCommand cmd = new OleDbCommand("Select * from [" + tableName + "]", excelConnection);

                excelConnection.Open();

                OleDbDataReader dReader;
                dReader = cmd.ExecuteReader();

                var List = new List<USER>();
                var listDegreeGraduate = new List<SpecificationGraduate>();
                var ListUserNotValid = new List<String>();
                while (dReader.Read())
                {
                    String Username = dReader[0].ToString();
                    String Passeword = dReader[1].ToString();
                    String Firstname = dReader[2].ToString();
                    String Lastname = dReader[3].ToString();
                    String Title = dReader[4].ToString();
                    String lenght = dReader[5].ToString();
                    String weight = dReader[6].ToString();
                    String averageGraduate = dReader[7].ToString();
     
                    if (Username.Length == 10)
                    {
                        var _exist = db.USERS.Where(u => u.Username == Username || u.Passeword == Passeword)
                            .FirstOrDefault();
                        if (_exist == null)
                        {
                            var user = new USER()
                            {
                                Username = Username,
                                Passeword = Passeword,
                                Comfirmepasseword = Passeword,
                                Lastname = Lastname,
                                Firstname = Firstname,
                                Title = Title,
                                Length = int.Parse(lenght),
                                Weight = int.Parse(weight),
                                AverageGraduate = float.Parse(averageGraduate)
                            };
                            var averageGraduateVm = new SpecificationGraduate
                            {
                                Length = int.Parse(lenght),
                                Weight = int.Parse(weight),
                                AverageGraduate = float.Parse(averageGraduate)
                            };
                            listDegreeGraduate.Add(averageGraduateVm);
                            List.Add(user);
                        }
                        else
                        {
                            ListUserNotValid.Add(Username);
                        }
                    }
                    else
                    {
                        ListUserNotValid.Add(Username);
                    }
                }

                foreach (var item in List)
                {
               
                    db.USERS.Add(item);
                    db.SaveChanges();
                }
                var ListUsers = db.USERS.ToList();
                foreach (var item in ListUsers)
                {
                    var specificationGraduate = new SpecificationGraduate
                    {
                        Length = item.Length,
                        Weight = item.Weight,
                        AverageGraduate = item.AverageGraduate,
                        User_Id = item.User_Id

                    };
                    db.SpecificationGraduate.Add(specificationGraduate);
                    db.SaveChanges();
                    // ADD ROLES TO INSERTED USERS
                    ROLE role = db.ROLES.Find(3);

                    USER userInserted = db.USERS.Find(item.User_Id);

                    if (!role.USERS.Contains(userInserted))
                    {
                        role.USERS.Add(userInserted);
                        db.SaveChanges();
                    }
                }
      







                excelConnection.Close();
                if (ListUserNotValid == null || ListUserNotValid.Count() == 0)
                {
                    message = "تم إضافة المستخدمين في  ملف Excel بنجاح!";
                    TempData["SuccessuploadedUserExcel"] = message;
                }
                else
                {
                    var listcin = "";
                    foreach (var item in ListUserNotValid)
                    {
                        listcin = listcin + " " + item + " , ";
                    }

                    ;
                    message =
                        "  هناك بعض المدخلات لم يتم إضافتها لتكرر الرقم العسكري , رقم الهوية او وجود رقم هوية لايتكون من 10ارقام: (" +
                        listcin + ")";
                    TempData["warninguploadedUserExcel"] = message;
                }
            }
            else
            {
                message = "لم تتم إضافة المستخدمين من ملف الإكسل !";
            }

            Debug.WriteLine("message" + message);
            return Json(message);
        }

        public ActionResult reset()
        {
            var users = db.USERS.Where(u => !u.Title.Equals("إداري")).ToList();
            foreach (var user in users)
            {
                db.USERS.Remove(user);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult UserInactive(int Id)
        {
            try
            {
                USER user = db.USERS.Find(Id);
                if (user != null)
                {
                    Debug.WriteLine("test" + user.User_Id);
                    db.Entry(user).Entity.Inactive = true;
                    db.Entry(user).Entity.User_Id = user.User_Id;
                    db.Entry(user).Entity.LastModified = System.DateTime.Now;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                }

                TempData["SuccessInactive"] = "تم إلغاء تفعيل المستخدم بنجاح!";
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                throw;
            }
        }

        // GET: Admin
        //return list users active (Inactive == false || null)
        public ActionResult Index()
        {
            return View(db.USERS.Where(r => r.Inactive == false || r.Inactive == null).OrderBy(r => r.Lastname)
                .ThenBy(r => r.Firstname).ToList());
        }

        //return list users Inactive (Inactive == true)
        public ActionResult Inactive()
        {
            return View(db.USERS.Where(r => r.Inactive == true).OrderBy(r => r.Lastname).ThenBy(r => r.Firstname)
                .ToList());
        }

        public ActionResult UserActive(int Id)
        {
            USER user = db.USERS.Find(Id);
            if (user != null)
            {
                db.Entry(user).Entity.Inactive = false;
                db.Entry(user).Entity.User_Id = user.User_Id;
                db.Entry(user).Entity.LastModified = System.DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            TempData["SuccessActive"] = "تم تفعيل المستخدم بنجاح!";
            return RedirectToAction("Index");
        }

        public ActionResult InsortableGraduate(int Id)
        {
            USER user = db.USERS.Find(Id);
            if (user != null)
            {
                db.Entry(user).Entity.Insortable = true;
                db.Entry(user).Entity.User_Id = user.User_Id;
                db.Entry(user).Entity.LastModified = System.DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            TempData["SuccesInsortable"] = "تم إستبعاد الخريج من عملية التوزيع بنجاح!";
            return RedirectToAction("Index", "GraduateArrangementVM");
        }

        public ActionResult ListGraduateInsortable()
        {
            ViewBag.sector = new SelectList(db.Sector, "IdSector", "NameSector");

            return View(db.USERS.Where(r => r.Inactive != true && r.Insortable == true && r.Title == "خريج")
                .Include(g => g.Sector).OrderBy(r => r.Lastname).ThenBy(r => r.Firstname).ToList());
        }

        //-----------------------------------------------------------
        [HttpPost]
        public void SortableG(int id, int IdSector)
        {
            try
            {
                USER Graduate = db.USERS.Find(id);
                if (Graduate != null)
                {
                    db.Entry(Graduate).Entity.IdSector = IdSector;
                    db.Entry(Graduate).Entity.LastModified = System.DateTime.Now;
                    db.Entry(Graduate).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
            }
        }

        //--------------------------------------------------------------
        public ActionResult ResortableGraduate(int Id)
        {
            USER user = db.USERS.Find(Id);
            if (user != null)
            {
                db.Entry(user).Entity.Insortable = false;
                db.Entry(user).Entity.User_Id = user.User_Id;
                db.Entry(user).Entity.LastModified = System.DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            TempData["SuccessResortable"] = "تم إعادة الخريج إلى عملية التوزيع بنجاح!";
            return RedirectToAction("Index", "GraduateArrangementVM");
        }

        public ActionResult GetListGraduateDegree()
        {
            var listGraduateDegree = (from t1 in db.SpecificationGraduate
                join t2 in db.USERS
                    on t1.User_Id equals t2.User_Id
                where t2.Title == "خريج"
                select (new {t2.Username, t2.Firstname, t2.Lastname, t2.Passeword, t2.User_Id})).ToList();
            List<USER> listGraduateDegreeview = new List<USER>();
            List<DetailsRequirementVM> degreeMatrial = new List<DetailsRequirementVM>();
            if (listGraduateDegree.Any())
            {
                foreach (var item in listGraduateDegree)
                {
                    var id = item.User_Id;
                    var Graduate = db.USERS.SingleOrDefault(u => u.User_Id == id);
                    if (Graduate == null || Graduate.Title != "خريج")
                    {
                        return RedirectToAction("ListGraduate", "Admin");
                    }
                    else
                    {
                        var Spesification = db.SpecificationGraduate.SingleOrDefault(m => m.User_Id == id);

                        var RequirementMaterial = db.DegreeGraduate.Include(m => m.Requirement)
                            .Where(m => (int) m.Requirement.Categorie == 0);
                        var listMaterial = RequirementMaterial.Where(m => m.User_Id == id).ToList();

                        var Requirementsuplement = db.DegreeGraduate.Include(m => m.Requirement)
                            .Where(m => (int) m.Requirement.Categorie == 3);
                        var listsuplement = Requirementsuplement.Where(m => m.User_Id == id).ToList();

                        var RequirementFitness = db.DegreeGraduate.Include(m => m.Requirement)
                            .Where(m => (int) m.Requirement.Categorie == 1);
                        var listFitness = RequirementFitness.Where(m => m.User_Id == id).ToList();

                        var Requirementmesure = db.DegreeGraduate.Include(m => m.Requirement)
                            .Where(m => (int) m.Requirement.Categorie == 2);
                        var listmesure = Requirementmesure.Where(m => m.User_Id == id).ToList();

                        var RequirementLeadership = db.LeadershipGraduate.Include(m => m.Leadershiprequirement);
                        var listLeadership = RequirementLeadership.Where(m => m.User_Id == id).ToList();

                        if (Spesification == null)
                        {
                            TempData["notadddegree"] = "لم يتم تسجيل درجات الخريج!";
                            return RedirectToAction("Create", "alldegreeGraduate",
                                new RouteValueDictionary(new {id = Graduate.User_Id}));
                        }
                        else
                        {
                            List<DetailsRequirementVM> listMaterialVM = new List<DetailsRequirementVM>();
                            List<DetailsRequirementVM> listsuplementVM = new List<DetailsRequirementVM>();
                            List<DetailsRequirementVM> listFitnessVM = new List<DetailsRequirementVM>();
                            List<DetailsRequirementVM> listmesureVM = new List<DetailsRequirementVM>();
                            List<DetailsLeadershipVM> listLeadershipVM = new List<DetailsLeadershipVM>();

                            foreach (var item1 in listMaterial)

                            {
                                var DetailsRequirementVM = new DetailsRequirementVM();
                                DetailsRequirementVM.NameRequirement = item1.Requirement.NameRequirement;
                                DetailsRequirementVM.percentage = item1.percentage;

                                listMaterialVM.Add(DetailsRequirementVM);
                            }

                            foreach (var item2 in listsuplement)

                            {
                                var DetailsRequirementVM = new DetailsRequirementVM();
                                DetailsRequirementVM.NameRequirement = item2.Requirement.NameRequirement;
                                DetailsRequirementVM.percentage = item2.percentage;

                                listsuplementVM.Add(DetailsRequirementVM);
                            }

                            foreach (var item3 in listFitness)

                            {
                                var DetailsRequirementVM = new DetailsRequirementVM();
                                DetailsRequirementVM.NameRequirement = item3.Requirement.NameRequirement;
                                DetailsRequirementVM.percentage = item3.percentage;

                                listFitnessVM.Add(DetailsRequirementVM);
                            }

                            foreach (var item4 in listmesure)

                            {
                                var DetailsRequirementVM = new DetailsRequirementVM();
                                DetailsRequirementVM.NameRequirement = item4.Requirement.NameRequirement;
                                DetailsRequirementVM.percentage = item4.percentage;

                                listmesureVM.Add(DetailsRequirementVM);
                            }

                            foreach (var item5 in listLeadership)

                            {
                                var DetailsLeadershipVM = new DetailsLeadershipVM();
                                DetailsLeadershipVM.NameLeadership = item5.Leadershiprequirement.NameLeadership;
                                DetailsLeadershipVM.IsChecked = item5.IsChecked;

                                listLeadershipVM.Add(DetailsLeadershipVM);
                            }

                            var rapportGraduate = new alldegreeGraduateVM
                            {
                                IdGraduate = Graduate.User_Id,
                                Firstname = Graduate.Firstname,
                                Lastname = Graduate.Lastname,
                                NumGraduate = Int64.Parse(Graduate.Passeword),
                                IdentityGraduate = Int64.Parse(Graduate.Username),
                                lenght = Spesification.Length,
                                weight = Spesification.Weight,
                                AverageGraduate = Spesification.AverageGraduate,
                                LeadershipVM = listLeadershipVM,
                                degreeMatrial = listMaterialVM,
                                degreesupplement = listsuplementVM,
                                degreeFitness = listFitnessVM,
                                degreeMesure = listmesureVM
                            };


                            var user = new USER
                            {
                                Username = item.Username,
                                Lastname = item.Lastname,
                                Firstname = item.Firstname,
                                Passeword = item.Passeword,
                                User_Id = item.User_Id,
                                RapportGraduate = rapportGraduate
                            };
                            listGraduateDegreeview.Add(user);
                        }
                    }
                }
            }

            return View(listGraduateDegreeview);
        }

        public ActionResult GetListGraduateSubjectMark()
        {
            var ListGraduateDegree = (from t1 in db.SpecificationGraduate
                join t2 in db.USERS
                    on t1.User_Id equals t2.User_Id
                where t2.Title == "خريج"
                select (new {t2.Username, t2.Firstname, t2.Lastname, t2.Passeword, t2.User_Id})).ToList();
            List<USER> ListGraduateDegreeview = new List<USER>();
            if (ListGraduateDegree != null)
            {
                foreach (var item in ListGraduateDegree)
                {
                    var user = new USER
                    {
                        Username = item.Username,
                        Lastname = item.Lastname,
                        Firstname = item.Firstname,
                        Passeword = item.Passeword,
                        User_Id = item.User_Id
                    };
                    ListGraduateDegreeview.Add(user);
                }
            }

            return View(ListGraduateDegreeview);
        }

        //public ActionResult GetListGraduateChoice()
        //{

        //}
        public ActionResult GetListGraduateNoNDegree()
        {
            var ListGraduateNoNDegree = (from t1 in db.USERS
                join t2 in db.SpecificationGraduate on t1.User_Id equals t2.User_Id into j
                from t2 in j.DefaultIfEmpty()
                where t2 == null && t1.Title == "خريج"
                select new {t1.User_Id, t1.Username, t1.Firstname, t1.Lastname, t1.Passeword}).ToList();
            List<USER> ListGraduateNoNDegreeview = new List<USER>();
            if (ListGraduateNoNDegree != null)
            {
                foreach (var item in ListGraduateNoNDegree)
                {
                    var user = new USER
                    {
                        Username = item.Username,
                        Lastname = item.Lastname,
                        Firstname = item.Firstname,
                        Passeword = item.Passeword,
                        User_Id = item.User_Id
                    };
                    ListGraduateNoNDegreeview.Add(user);
                }
            }

            return View(ListGraduateNoNDegreeview);
        }

        public ActionResult GetListGraduateWishes()
        {
            var listGraduateWishes = (from t1 in db.USERS
                    join t2 in db.GraduateWishes
                        on t1.User_Id equals t2.User_Id
                    where t1.Title == "خريج"
                    select (new {t1.User_Id, t1.Username, t1.Firstname, t1.Lastname, t1.Passeword})).Distinct()
                .ToList();

            List<USER> listGraduateWishesview = new List<USER>();
            List<GraduateWishes> usersWishes = new List<GraduateWishes>();
            if (listGraduateWishes.Any())
            {
                foreach (var item in listGraduateWishes)
                {
                    if (item.User_Id == 0)
                    {
                        var graduateWishes = db.GraduateWishes.Include(g => g.USER).Include(g => g.Sector)
                            .Where(g => g.User_Id == item.User_Id);
                        var graduate = db.USERS.Find(item.User_Id);
                        if (graduate != null)
                        {
                            ViewBag.NameGraduate = graduate.Lastname + " " + graduate.Firstname;
                            ViewBag.Passeword = graduate.Passeword;
                            ViewBag.userName = graduate.Username;
                        }

                        usersWishes = graduateWishes.OrderBy(g => g.Rank).ToList();
                    }
                    else
                    {
                        var graduateWishes = db.GraduateWishes.Include(g => g.USER).Include(g => g.Sector)
                            .Where(g => g.User_Id == item.User_Id);
                        if (graduateWishes.Any())
                        {
                            usersWishes = graduateWishes.OrderBy(g => g.Rank).ToList();
                        }
                    }


                    var user = new USER
                    {
                        Username = item.Username,
                        Lastname = item.Lastname,
                        Firstname = item.Firstname,
                        Passeword = item.Passeword,
                        User_Id = item.User_Id,
                        GraduateWishes = usersWishes
                    };

                    listGraduateWishesview.Add(user);
                }
            }

            return View(listGraduateWishesview);
        }

        public ActionResult GetListGraduateNoNWishes()
        {
            var ListGraduateNoNWishes = (from t1 in db.USERS
                join t2 in db.GraduateWishes on t1.User_Id equals t2.User_Id into j
                from t2 in j.DefaultIfEmpty()
                where t2 == null && t1.Title == "خريج"
                select new {t1.User_Id, t1.Username, t1.Firstname, t1.Lastname, t1.Passeword}).ToList();
            List<USER> ListGraduateNoNWishesview = new List<USER>();
            if (ListGraduateNoNWishes != null)
            {
                foreach (var item in ListGraduateNoNWishes)
                {
                    var user = new USER
                    {
                        Username = item.Username,
                        Lastname = item.Lastname,
                        Firstname = item.Firstname,
                        Passeword = item.Passeword,
                        User_Id = item.User_Id
                    };

                    ListGraduateNoNWishesview.Add(user);
                }
            }

            return View(ListGraduateNoNWishesview);
        }

        public ActionResult UserDelete(int id)
        {
            USER _user = db.USERS.Find(id);
            if (_user != null)
            {
                _user.ROLES.Clear();


                db.Entry(_user).State = EntityState.Deleted;
                db.SaveChanges();
            }

            TempData["SuccessUserDelete"] = "تم حذف المستخدم بنجاح!";
            return RedirectToAction("Index");
        }

        public ViewResult UserDetails(int id)
        {
            USER user = db.USERS.Find(id);
            SetViewBagData(id);
            return View(user);
        }

        public ActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserCreate(USER user)
        {
            if (user.Username == "" || user.Username == null)
            {
                ModelState.AddModelError(string.Empty, "رقم الهوية يجب أن لا يكون فارغاً");
            }

            if (user.Passeword == "" || user.Passeword == null)
            {
                ModelState.AddModelError(string.Empty, "الرقم العسكري يجب أن لا يكون فارغاً");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    List<string> results = db.Database
                        .SqlQuery<String>(string.Format("SELECT Username FROM USERS WHERE Username = '{0}'",
                            user.Username)).ToList();
                    bool _userExistsInTable = (results.Count > 0);

                    USER _user = null;
                    if (_userExistsInTable)
                    {
                        _user = db.USERS.Where(p => p.Username == user.Username).FirstOrDefault();
                        if (_user != null)
                        {
                            if (_user.Inactive == false)
                            {
                                if (_user.Passeword == user.Passeword)
                                {
                                    ModelState.AddModelError(string.Empty, "هذا المستخدم موجود!");
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty,
                                        "  هذا المستخدم موجود، الرجاء التأكد من الرقم العسكري!");
                                }
                            }
                            else
                            {
                                if (_user.Passeword == user.Passeword)
                                {
                                    db.Entry(_user).Entity.Inactive = false;
                                    db.Entry(_user).Entity.LastModified = System.DateTime.Now;
                                    db.Entry(_user).State = EntityState.Modified;
                                    db.SaveChanges();
                                    TempData["Successactiveuser"] = "تم تفعيل المستخدم بنجاح!";
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty,
                                        "  هذا المستخدم موجود لكنه غير مفعل، الرجاء التأكد من الرقم العسكري");
                                }
                            }
                        }
                    }
                    else
                    {
                        _user = db.USERS.Where(p => p.Passeword == user.Passeword).FirstOrDefault();

                        if (_user != null)
                        {
                            if (_user.Inactive == false)
                            {
                                ModelState.AddModelError(string.Empty,
                                    "  الرقم العسكري موجود وغير مطابقة رقم الهوية");
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty,
                                    " هذا المستخدم غير مفعل ،الرقم العسكري موجود وغير مطابق لرقم الهوية");
                            }
                        }
                        else
                        {
                            _user = new USER
                            {
                                Username = user.Username,
                                Lastname = user.Lastname,
                                Firstname = user.Firstname,
                                Title = user.Title,
                                Passeword = user.Passeword,
                                Comfirmepasseword = user.Comfirmepasseword,
                                Inactive = user.Inactive
                            };


                            if (ModelState.IsValid)
                            {
                                _user.Inactive = false;
                                _user.LastModified = System.DateTime.Now;

                                db.USERS.Add(_user);
                                db.SaveChanges();
                                TempData["Successadduser"] = "تمت إضافة المستخدم بنجاح!";
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //return base.ShowError(ex);
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult UserEdit(int id)
        {
            USER user = db.USERS.Find(id);
            SetViewBagData(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult UserEdit(USER user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.Entry(user).Entity.LastModified = System.DateTime.Now;

                db.SaveChanges();

                TempData["Successupdate"] = "تم تعديل المستخدم بنجاح!";
                return RedirectToAction("UserDetails", new RouteValueDictionary(new {id = user.User_Id}));
            }
            else
            {
                TempData["errorupdate"] = "خطأ تعديل المستخدم!";
                return RedirectToAction("UserEdit", new RouteValueDictionary(new {id = user.User_Id}));
            }


            //USER _user = db.USERS.Where(p => p.User_Id == user.User_Id).FirstOrDefault();
            //if (_user != null)
            //{
            //    try
            //    {
            //        db.Entry(_user).CurrentValues.SetValues(user);
            //        db.Entry(_user).Entity.Title = user.Title;
            //        db.Entry(_user).Entity.LastModified = System.DateTime.Now;
            //        db.SaveChanges();
            //    }
            //    catch (Exception)
            //    {

            //    }
            //}
            //TempData["Success"] = "تم تعديل المستخدم بنجاح!";
            //return RedirectToAction("UserDetails", new RouteValueDictionary(new { id = user.User_Id }));
        }

        [HttpPost]
        public ActionResult UserDetails(USER user)
        {
            if (user.Username == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid USER Name");
            }

            if (ModelState.IsValid)
            {
                db.Entry(user).Entity.Inactive = user.Inactive;
                db.Entry(user).Entity.LastModified = System.DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult DeleteUserRole(int id, int userId)
        {
            ROLE role = db.ROLES.Find(id);
            USER user = db.USERS.Find(userId);

            if (role.USERS.Contains(user))
            {
                role.USERS.Remove(user);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home", new {id = userId});
        }

        [HttpGet]
        public PartialViewResult filter4Users(string _surname)
        {
            return PartialView("_ListUserTable", GetFilteredUserList(_surname));
        }

        [HttpGet]
        public PartialViewResult filterReset()
        {
            return PartialView("_ListUserTable",
                db.USERS.Where(r => r.Inactive == false || r.Inactive == null).ToList());
        }

        [HttpGet]
        public PartialViewResult DeleteUserReturnPartialView(int userId)
        {
            try
            {
                USER user = db.USERS.Find(userId);
                if (user != null)
                {
                    db.Entry(user).Entity.Inactive = true;
                    db.Entry(user).Entity.User_Id = user.User_Id;
                    db.Entry(user).Entity.LastModified = System.DateTime.Now;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch
            {
            }

            return this.filterReset();
        }


        private IEnumerable<USER> GetFilteredUserList(string _surname)
        {
            IEnumerable<USER> _ret = null;
            try
            {
                if (string.IsNullOrEmpty(_surname))
                {
                    _ret = db.USERS.Where(r => r.Inactive == false || r.Inactive == null).ToList();
                }
                else
                {
                    _ret = db.USERS.Where(p =>
                        p.Username == _surname || p.Firstname == _surname || p.Lastname == _surname).ToList();
                }
            }
            catch
            {
            }

            return _ret;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteUserRoleReturnPartialView(int id, int userId)
        {
            ROLE role = db.ROLES.Find(id);
            USER user = db.USERS.Find(userId);

            if (role.USERS.Contains(user))
            {
                role.USERS.Remove(user);
                db.SaveChanges();
            }

            SetViewBagData(userId);
            return PartialView("_ListUserRoleTable", db.USERS.Find(userId));
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUserRoleReturnPartialView(int id, int userId)
        {
            ROLE role = db.ROLES.Find(id);
            USER user = db.USERS.Find(userId);

            if (!role.USERS.Contains(user))
            {
                role.USERS.Add(user);
                db.SaveChanges();
            }

            SetViewBagData(userId);
            return PartialView("_ListUserRoleTable", db.USERS.Find(userId));
        }

        private void SetViewBagData(int _userId)
        {
            ViewBag.UserId = _userId;
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            ViewBag.RoleId = new SelectList(db.ROLES.OrderBy(p => p.RoleName), "Role_Id", "RoleName");
        }

        public List<SelectListItem> List_boolNullYesNo()
        {
            var _retVal = new List<SelectListItem>();
            try
            {
                _retVal.Add(new SelectListItem {Text = "Not Set", Value = null});
                _retVal.Add(new SelectListItem {Text = "Yes", Value = bool.TrueString});
                _retVal.Add(new SelectListItem {Text = "No", Value = bool.FalseString});
            }
            catch
            {
            }

            return _retVal;
        }

        public ActionResult ListGraduate()
        {
            return View(db.USERS.Where(r => r.Title == "خريج" && (r.Inactive == false || r.Inactive == null))
                .OrderBy(r => r.Lastname).ThenBy(r => r.Firstname).ToList());
        }

        #endregion

        #region ROLES

        public ActionResult RoleIndex()
        {
            return View(db.ROLES.OrderBy(r => r.RoleDescription).ToList());
        }

        public ViewResult RoleDetails(int id)
        {
            USER user = db.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            ROLE role = db.ROLES.Where(r => r.Role_Id == id)
                .Include(a => a.PERMISSIONS)
                .Include(a => a.USERS)
                .FirstOrDefault();

            // USERS combo
            ViewBag.UserId = new SelectList(db.USERS.Where(r => r.Inactive == false || r.Inactive == null), "Id",
                "Username");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(db.PERMISSIONS.OrderBy(a => a.PermissionDescription), "Permission_Id",
                "PermissionName");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(role);
        }

        public ActionResult RoleCreate()
        {
            USER user = db.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View();
        }

        [HttpPost]
        public ActionResult RoleCreate(ROLE _role)
        {
            if (_role.RoleDescription == null)
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            USER user = db.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                db.ROLES.Add(_role);
                db.SaveChanges();
                return RedirectToAction("RoleIndex");
            }

            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }


        public ActionResult RoleEdit(int id)
        {
            USER user = db.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();

            ROLE _role = db.ROLES.Where(r => r.Role_Id == id)
                .Include(a => a.PERMISSIONS)
                .Include(a => a.USERS)
                .FirstOrDefault();

            // USERS combo
            ViewBag.UserId = new SelectList(db.USERS.Where(r => r.Inactive == false || r.Inactive == null), "User_Id",
                "Username");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(db.PERMISSIONS.OrderBy(a => a.Permission_Id), "Permission_Id",
                "PermissionName");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(_role);
        }

        [HttpPost]
        public ActionResult RoleEdit(ROLE _role)
        {
            if (string.IsNullOrEmpty(_role.RoleDescription))
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            //EntityState state = db.Entry(_role).State;
            USER user = db.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                db.Entry(_role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RoleDetails", new RouteValueDictionary(new {id = _role.Role_Id}));
            }

            // USERS combo
            ViewBag.UserId = new SelectList(db.USERS.Where(r => r.Inactive == false || r.Inactive == null), "User_Id",
                "Username");

            // Rights combo
            ViewBag.PermissionId = new SelectList(db.PERMISSIONS.OrderBy(a => a.Permission_Id), "Permission_Id",
                "PermissionName");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }


        public ActionResult RoleDelete(int id)
        {
            ROLE _role = db.ROLES.Find(id);
            if (_role != null)
            {
                _role.USERS.Clear();
                _role.PERMISSIONS.Clear();

                db.Entry(_role).State = EntityState.Deleted;
                db.SaveChanges();
            }

            return RedirectToAction("RoleIndex");
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteUserFromRoleReturnPartialView(int id, int userId)
        {
            ROLE role = db.ROLES.Find(id);
            USER user = db.USERS.Find(userId);

            if (role.USERS.Contains(user))
            {
                role.USERS.Remove(user);
                db.SaveChanges();
            }

            return PartialView("_ListUsersTable4Role", role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUser2RoleReturnPartialView(int id, int userId)
        {
            ROLE role = db.ROLES.Find(id);
            USER user = db.USERS.Find(userId);

            if (!role.USERS.Contains(user))
            {
                role.USERS.Add(user);
                db.SaveChanges();
            }

            return PartialView("_ListUsersTable4Role", role);
        }

        #endregion

        #region PERMISSIONS

        public ViewResult PermissionIndex()
        {
            List<PERMISSION> _permissions = db.PERMISSIONS
                .OrderBy(wn => wn.PermissionDescription)
                .Include(a => a.ROLES)
                .ToList();
            return View(_permissions);
        }

        public ViewResult PermissionDetails(int id)
        {
            PERMISSION _permission = db.PERMISSIONS.Find(id);
            return View(_permission);
        }

        public ActionResult PermissionCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PermissionCreate(PERMISSION _permission)
        {
            if (_permission.PermissionDescription == null)
            {
                ModelState.AddModelError("Permission Description", "Permission Description must be entered");
            }

            if (ModelState.IsValid)
            {
                db.PERMISSIONS.Add(_permission);
                db.SaveChanges();
                return RedirectToAction("PermissionIndex");
            }

            return View(_permission);
        }

        public ActionResult PermissionEdit(int id)
        {
            PERMISSION _permission = db.PERMISSIONS.Find(id);
            ViewBag.RoleId = new SelectList(db.ROLES.OrderBy(p => p.RoleDescription), "Role_Id", "RoleName");
            return View(_permission);
        }

        [HttpPost]
        public ActionResult PermissionEdit(PERMISSION _permission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(_permission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PermissionDetails",
                    new RouteValueDictionary(new {id = _permission.Permission_Id}));
            }

            return View(_permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult PermissionDelete(int id)
        {
            PERMISSION permission = db.PERMISSIONS.Find(id);
            if (permission.ROLES.Count > 0)
                permission.ROLES.Clear();

            db.Entry(permission).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("PermissionIndex");
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddPermission2RoleReturnPartialView(int id, int permissionId)
        {
            ROLE role = db.ROLES.Find(id);
            PERMISSION _permission = db.PERMISSIONS.Find(permissionId);

            if (!role.PERMISSIONS.Contains(_permission))
            {
                role.PERMISSIONS.Add(_permission);
                db.SaveChanges();
            }

            return PartialView("_ListPermissions", role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddAllPermissions2RoleReturnPartialView(int id)
        {
            ROLE _role = db.ROLES.Where(p => p.Role_Id == id).FirstOrDefault();
            List<PERMISSION> _permissions = db.PERMISSIONS.ToList();
            foreach (PERMISSION _permission in _permissions)
            {
                if (!_role.PERMISSIONS.Contains(_permission))
                {
                    _role.PERMISSIONS.Add(_permission);
                }
            }

            db.SaveChanges();
            return PartialView("_ListPermissions", _role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeletePermissionFromRoleReturnPartialView(int id, int permissionId)
        {
            ROLE _role = db.ROLES.Find(id);
            PERMISSION _permission = db.PERMISSIONS.Find(permissionId);

            if (_role.PERMISSIONS.Contains(_permission))
            {
                _role.PERMISSIONS.Remove(_permission);
                db.SaveChanges();
            }

            return PartialView("_ListPermissions", _role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteRoleFromPermissionReturnPartialView(int id, int permissionId)
        {
            ROLE role = db.ROLES.Find(id);
            PERMISSION permission = db.PERMISSIONS.Find(permissionId);

            if (role.PERMISSIONS.Contains(permission))
            {
                role.PERMISSIONS.Remove(permission);
                db.SaveChanges();
            }

            return PartialView("_ListRolesTable4Permission", permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddRole2PermissionReturnPartialView(int permissionId, int roleId)
        {
            ROLE role = db.ROLES.Find(roleId);
            PERMISSION _permission = db.PERMISSIONS.Find(permissionId);

            if (!role.PERMISSIONS.Contains(_permission))
            {
                role.PERMISSIONS.Add(_permission);
                db.SaveChanges();
            }

            return PartialView("_ListRolesTable4Permission", _permission);
        }

        public ActionResult PermissionsImport()
        {
            var _controllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t != null
                            && t.IsPublic
                            && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                            && !t.IsAbstract
                            && typeof(IController).IsAssignableFrom(t));

            var _controllerMethods = _controllerTypes.ToDictionary(controllerType => controllerType,
                controllerType => controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => typeof(ActionResult).IsAssignableFrom(m.ReturnType)));

            foreach (var _controller in _controllerMethods)
            {
                string _controllerName = _controller.Key.Name;
                foreach (var _controllerAction in _controller.Value)
                {
                    string _controllerActionName = _controllerAction.Name;
                    if (_controllerName.EndsWith("Controller"))
                    {
                        _controllerName = _controllerName.Substring(0, _controllerName.LastIndexOf("Controller"));
                    }

                    string _permissionDescription = string.Format("{0}-{1}", _controllerName, _controllerActionName);
                    PERMISSION _permission = db.PERMISSIONS
                        .Where(p => p.PermissionDescription == _permissionDescription).FirstOrDefault();
                    if (_permission == null)
                    {
                        if (ModelState.IsValid)
                        {
                            PERMISSION _perm = new PERMISSION {PermissionDescription = _permissionDescription};

                            db.PERMISSIONS.Add(_perm);
                            db.SaveChanges();
                        }
                    }
                }
            }

            return RedirectToAction("PermissionIndex");
        }

        #endregion
    }
}