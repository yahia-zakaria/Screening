using Studentscreeningsystem.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;
using Studentscreeningsystem.ViewsModel;
using System.Web.Routing;
using System.Net;
using System.Data.Entity.Validation;

namespace Studentscreeningsystem.Controllers
{
    public class RequirementSectorVMController : Controller
    {
        private RBAC_Model db = new RBAC_Model();
       
        public ActionResult Create(int id)

        {
            IList<Requirement> RequirementMaterial = db.Requirement.Where(a => (int)a.Categorie == 0).ToList();
            IList<Requirement> Requirementsuplement = db.Requirement.Where(a => (int)a.Categorie == 3).ToList();
            IList<Requirement> RequirementFitness = db.Requirement.Where(a => (int)a.Categorie == 1).ToList();
            IList<Requirement> Requirementmesure = db.Requirement.Where(a => (int)a.Categorie == 2).ToList();
            IList<Leadershiprequirement> RequirementLeadership = db.Leadershiprequirement.ToList();
           
            ViewBag.RequirementMaterial = RequirementMaterial;
            ViewBag.Requirementsuplement = Requirementsuplement;
            ViewBag.RequirementFitness = RequirementFitness;
            ViewBag.Requirementmesure = Requirementmesure;
            ViewBag.RequirementLeadership = RequirementLeadership;
            //ViewBag.IdSector = new SelectList(db.Sector, "IdSector", "NameSector");
            ViewBag.Sector = db.Sector.Find(id);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RequirementSectorVM RequirementSectorVM,int id)
        {
            if (ModelState.IsValid)
            {
                var Specification = new Specification
                {
                    IdSector = RequirementSectorVM.IdSector,
                    Maxlenght = RequirementSectorVM.Maxlenght,
                    Minlenght = RequirementSectorVM.Minlenght,
                    Maxweight = RequirementSectorVM.Maxweight,
                    Minweight = RequirementSectorVM.Minweight
                };
                db.Specification.Add(Specification);
                db.SaveChanges();

                foreach (var item in RequirementSectorVM.Requirementsupplement)
                {
                    var RequirementSector = new RequirementSector
                    {
                        IdSector = RequirementSectorVM.IdSector,
                        IdRequirement = item.IdRequirement,
                        percentage = item.percentage
                    };
                    //RequirementSector.priority = item.priority;

                    db.RequirementSector.Add(RequirementSector);
                    db.SaveChanges();

                }

                foreach (var item in RequirementSectorVM.RequirementMatrial)
                {
                    var RequirementSector = new RequirementSector
                    {
                        IdSector = RequirementSectorVM.IdSector,
                        IdRequirement = item.IdRequirement,
                        percentage = item.percentage
                    };
                    //RequirementSector.priority = item.priority;

                    db.RequirementSector.Add(RequirementSector);
                    db.SaveChanges();

                }


                foreach (var item in RequirementSectorVM.RequirementFitness)
                {
                    var RequirementSector = new RequirementSector
                    {
                        IdSector = RequirementSectorVM.IdSector,
                        IdRequirement = item.IdRequirement,
                        percentage = item.percentage
                    };
                    //RequirementSector.priority = item.priority;

                    db.RequirementSector.Add(RequirementSector);
                    db.SaveChanges();

                }

                foreach (var item in RequirementSectorVM.RequirementMesure)
                {
                    var RequirementSector = new RequirementSector
                    {
                        IdSector = RequirementSectorVM.IdSector,
                        IdRequirement = item.IdRequirement,
                        percentage = item.percentage
                    };
                    //RequirementSector.priority = item.priority;

                    db.RequirementSector.Add(RequirementSector);
                    db.SaveChanges();

                }

                foreach (var item in RequirementSectorVM.LeadershipVM)
                {
                    var LeadershipSector = new LeadershipSector
                    {
                        IdSector = RequirementSectorVM.IdSector,
                        IdLeadership = item.IdLeadership,
                        IsChecked = item.IsChecked
                    };


                    db.LeadershipSector.Add(LeadershipSector);
                    db.SaveChanges();

                }


                db.SaveChanges();
                var update = db.Sector.Find(RequirementSectorVM.IdSector);
                update.NbGraduates = RequirementSectorVM.NbGraduates;
                db.SaveChanges();
                TempData["SuccesAddSector"] = "لقد تم إضافة السلاح و متطلباته بنجاح";
                return RedirectToAction("Index", "Sectors");
            }

            IList<Requirement> RequirementMaterial = db.Requirement.Where(a => (int)a.Categorie == 0).ToList();
            IList<Requirement> Requirementsuplement = db.Requirement.Where(a => (int)a.Categorie == 3).ToList();
            IList<Requirement> RequirementFitness = db.Requirement.Where(a => (int)a.Categorie == 1).ToList();
            IList<Requirement> Requirementmesure = db.Requirement.Where(a => (int)a.Categorie == 2).ToList();
            IList<Leadershiprequirement> RequirementLeadership = db.Leadershiprequirement.ToList();

            ViewBag.RequirementMaterial = RequirementMaterial;
            ViewBag.Requirementsuplement = Requirementsuplement;
            ViewBag.RequirementFitness = RequirementFitness;
            ViewBag.Requirementmesure = Requirementmesure;
            ViewBag.RequirementLeadership = RequirementLeadership;
            //ViewBag.IdSector = new SelectList(db.Sector, "IdSector", "NameSector");
            ViewBag.Sector = db.Sector.Find(id);
            return View(RequirementSectorVM);
        }

        public ActionResult RaportsSector(int id)
        {
            var Sector = db.Sector.Find(id);
            var Spesification = db.Specification.Where(m => m.IdSector == id).DefaultIfEmpty().Single();

            var RequirementMaterial = db.RequirementSector.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie ==0);
            var listMaterial = RequirementMaterial.Where(m => m.IdSector == id).ToList();

            var Requirementsuplement = db.RequirementSector.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 3);
            var listsuplement = Requirementsuplement.Where(m => m.IdSector == id).ToList();

            var RequirementFitness = db.RequirementSector.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 1);
            var listFitness = RequirementFitness.Where(m => m.IdSector == id).ToList();

            var Requirementmesure = db.RequirementSector.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 2);
            var listmesure = Requirementmesure.Where(m => m.IdSector == id).ToList();

            var RequirementLeadership = db.LeadershipSector.Include(m => m.Leadershiprequirement);
            var listLeadership = RequirementLeadership.Where(m => m.IdSector == id).ToList();

            List<DetailsRequirementVM> listMaterialVM = new List<DetailsRequirementVM>();
            List<DetailsRequirementVM> listsuplementVM = new List<DetailsRequirementVM>();
            List<DetailsRequirementVM> listFitnessVM = new List<DetailsRequirementVM>();
            List<DetailsRequirementVM> listmesureVM = new List<DetailsRequirementVM>();
            List<DetailsLeadershipVM> listLeadershipVM = new List<DetailsLeadershipVM>();

            if(Spesification!=null )
            {
                foreach (var item in listMaterial)

                {
                    var DetailsRequirementVM = new DetailsRequirementVM
                    {
                        NameRequirement = item.Requirement.NameRequirement, percentage = item.percentage
                    };
                    //DetailsRequirementVM.priority = item.priority;
                    listMaterialVM.Add(DetailsRequirementVM);

                }

                foreach (var item in listsuplement)

                {
                    var DetailsRequirementVM = new DetailsRequirementVM
                    {
                        NameRequirement = item.Requirement.NameRequirement, percentage = item.percentage
                    };
                    //DetailsRequirementVM.priority = item.priority;
                    listsuplementVM.Add(DetailsRequirementVM);

                }

                foreach (var item in listFitness)

                {
                    var DetailsRequirementVM = new DetailsRequirementVM
                    {
                        NameRequirement = item.Requirement.NameRequirement, percentage = item.percentage
                    };
                    //DetailsRequirementVM.priority = item.priority;
                    listFitnessVM.Add(DetailsRequirementVM);

                }

                foreach (var item in listmesure)

                {
                    var DetailsRequirementVM = new DetailsRequirementVM
                    {
                        NameRequirement = item.Requirement.NameRequirement, percentage = item.percentage
                    };
                    //DetailsRequirementVM.priority = item.priority;
                    listmesureVM.Add(DetailsRequirementVM);

                }

                foreach (var item in listLeadership)

                {
                    var DetailsLeadershipVM = new DetailsLeadershipVM
                    {
                        NameLeadership = item.Leadershiprequirement.NameLeadership, IsChecked = item.IsChecked
                    };

                    listLeadershipVM.Add(DetailsLeadershipVM);

                }

                var RapportSector = new RapportSectorVM
                {
                    IdSector = Sector.IdSector,
                    NameSector = Sector.NameSector,
                    NbGraduates = Sector.NbGraduates,
                    LogoPath= Sector.LogoPath,
                    Maxlenght = Spesification.Maxlenght,
                    Minlenght = Spesification.Minlenght,
                    Maxweight = Spesification.Maxweight,
                    Minweight = Spesification.Minweight,

                    RequirementLeadershipVM = listLeadershipVM,
                    RequirementMatrial = listMaterialVM,
                    Requirementsupplement = listsuplementVM,
                    RequirementFitness = listFitnessVM,
                    RequirementMesure = listmesureVM

                };
                return View(RapportSector);
            }
            else
            {
                TempData["NotcompleateDS"] = "عذراً ، لم تكمل إدخال تفاصيل السلاح !";
                return RedirectToAction("Create", "RequirementSectorVM", new RouteValueDictionary(new { id = Sector.IdSector}));


            }
          
        }

        public ActionResult EditSector(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Sector = db.Sector.Find(id);
            var Spesification = db.Specification.Where(m => m.IdSector == id).DefaultIfEmpty().Single();

            var RequirementMaterial = db.RequirementSector.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 0);
            var listMaterial = RequirementMaterial.Where(m => m.IdSector == id).ToList();

            var Requirementsuplement = db.RequirementSector.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 3);
            var listsuplement = Requirementsuplement.Where(m => m.IdSector == id).ToList();

            var RequirementFitness = db.RequirementSector.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 1);
            var listFitness = RequirementFitness.Where(m => m.IdSector == id).ToList();

            var Requirementmesure = db.RequirementSector.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 2);
            var listmesure = Requirementmesure.Where(m => m.IdSector == id).ToList();

            var RequirementLeadership = db.LeadershipSector.Include(m => m.Leadershiprequirement);
            var listLeadership = RequirementLeadership.Where(m => m.IdSector == id).ToList();

            if (Sector == null)
            {
                return RedirectToAction("Create","Sectors");
            }
            else
            {
                if (Spesification==null)
                {
                    TempData["NotcompleateDS"] = "عذراً ، لم تكمل إدخال تفاصيل السلاح !";
                    return RedirectToAction("Create", "RequirementSectorVM", new RouteValueDictionary(new { id = Sector.IdSector }));
                }
                else
                {
                    

                    List<DetailsRequirementVM> listMaterialVM = new List<DetailsRequirementVM>();
                    List<DetailsRequirementVM> listsuplementVM = new List<DetailsRequirementVM>();
                    List<DetailsRequirementVM> listFitnessVM = new List<DetailsRequirementVM>();
                    List<DetailsRequirementVM> listmesureVM = new List<DetailsRequirementVM>();
                    List<DetailsLeadershipVM> listLeadershipVM = new List<DetailsLeadershipVM>();
                    foreach (var item in listMaterial)

                    {
                        var DetailsRequirementVM = new DetailsRequirementVM
                        {
                            Id = item.Id,
                            IdRequirement = item.Requirement.IdRequirement,
                            NameRequirement = item.Requirement.NameRequirement,
                            percentage = item.percentage
                        };
                        //DetailsRequirementVM.priority = item.priority;
                        listMaterialVM.Add(DetailsRequirementVM);

                    }

                    foreach (var item in listsuplement)

                    {
                        var DetailsRequirementVM = new DetailsRequirementVM
                        {
                            Id = item.Id,
                            IdRequirement = item.Requirement.IdRequirement,
                            NameRequirement = item.Requirement.NameRequirement,
                            percentage = item.percentage
                        };
                        //DetailsRequirementVM.priority = item.priority;
                        listsuplementVM.Add(DetailsRequirementVM);

                    }

                    foreach (var item in listFitness)

                    {
                        var DetailsRequirementVM = new DetailsRequirementVM
                        {
                            Id = item.Id,
                            IdRequirement = item.Requirement.IdRequirement,
                            NameRequirement = item.Requirement.NameRequirement,
                            percentage = item.percentage
                        };
                        //DetailsRequirementVM.priority = item.priority;
                        listFitnessVM.Add(DetailsRequirementVM);

                    }

                    foreach (var item in listmesure)

                    {
                        var DetailsRequirementVM = new DetailsRequirementVM
                        {
                            Id = item.Id,
                            IdRequirement = item.Requirement.IdRequirement,
                            NameRequirement = item.Requirement.NameRequirement,
                            percentage = item.percentage
                        };
                        //DetailsRequirementVM.priority = item.priority;
                        listmesureVM.Add(DetailsRequirementVM);

                    }

                    foreach (var item in listLeadership)

                    {
                        var DetailsLeadershipVM = new DetailsLeadershipVM
                        {
                            Id = item.Id,
                            NameLeadership = item.Leadershiprequirement.NameLeadership,
                            IsChecked = item.IsChecked
                        };

                        listLeadershipVM.Add(DetailsLeadershipVM);

                    }

                    var RapportSector = new RapportSectorVM
                    {
                        IdSector = Sector.IdSector,
                        NameSector = Sector.NameSector,
                        NbGraduates = Sector.NbGraduates,
                        LogoPath = Sector.LogoPath,
                        Maxlenght = Spesification.Maxlenght,
                        Minlenght = Spesification.Minlenght,
                        Maxweight = Spesification.Maxweight,
                        Minweight = Spesification.Minweight,
                        Idspesification= Spesification.IdSpecification,
                        RequirementLeadershipVM = listLeadershipVM,
                        RequirementMatrial = listMaterialVM,
                        Requirementsupplement = listsuplementVM,
                        RequirementFitness = listFitnessVM,
                        RequirementMesure = listmesureVM

                    };
                    return View(RapportSector);
                }
            }
          
        }

        // POST: Sectors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSector(RapportSectorVM RapportSector, HttpPostedFileBase LogoPath)
        {
            if (ModelState.IsValid)
            {
                var Specification = db.Specification.Find(RapportSector.Idspesification);
                Specification.Maxlenght = RapportSector.Maxlenght;
                Specification.Minlenght = RapportSector.Minlenght;
                Specification.Maxweight = RapportSector.Maxweight;
                Specification.Minweight = RapportSector.Minweight;
                db.Entry(Specification).State = EntityState.Modified;
                db.SaveChanges();

                var sector = db.Sector.Find(RapportSector.IdSector);
                sector.NameSector = RapportSector.NameSector;
                sector.NbGraduates = RapportSector.NbGraduates;
                if (LogoPath == null) {
                    RapportSector.LogoPath = sector.LogoPath; }
                else { RapportSector.LogoPath = LogoPath.FileName; }
                
                    if (sector.LogoPath != RapportSector.LogoPath)
                    {
                        if (LogoPath.ContentType == "image/jpg" || LogoPath.ContentType == "image/png" || LogoPath.ContentType == "image/bmp" || LogoPath.ContentType == "image/gif" || LogoPath.ContentType == "image/jpeg")
                        {
                        LogoPath.SaveAs(Server.MapPath("/") + "/SectorsLogo/" + LogoPath.FileName);
                        }
                    }
               

                sector.LogoPath = RapportSector.LogoPath;
                db.SaveChanges();

                foreach (var item in RapportSector.RequirementMatrial)
                {
                    try
                    {
                        RequirementSector RequirementSector = db.RequirementSector.Find(item.Id);
                        RequirementSector.Id = item.Id;
                        RequirementSector.IdSector = RapportSector.IdSector;
                        RequirementSector.IdRequirement = item.IdRequirement;
                        RequirementSector.percentage = item.percentage;
                        //RequirementSector.priority = item.priority;


                        db.Entry(RequirementSector).State = EntityState.Modified;

                        db.SaveChanges();

                    }
    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting  
                            // the current instance as InnerException  
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }


            }
                foreach (var item in RapportSector.RequirementMesure)
                {
                    try
                    {
                        var matrial = db.RequirementSector.Find(item.Id);
                    matrial.Id = item.Id;
                    matrial.IdSector = RapportSector.IdSector;
                    matrial.IdRequirement = item.IdRequirement;
                    matrial.percentage = item.percentage;
                    //matrial.priority = item.priority;
                    db.Entry(matrial).State = EntityState.Modified;
                    db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting  
                                // the current instance as InnerException  
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }

                }
                if (RapportSector.Requirementsupplement!=null) {
                    foreach (var item in RapportSector.Requirementsupplement)
                    {
                        try
                        {
                            var matrial = db.RequirementSector.Find(item.Id);
                            matrial.Id = item.Id;
                            matrial.IdSector = RapportSector.IdSector;
                            matrial.IdRequirement = item.IdRequirement;
                            matrial.percentage = item.percentage;
                            //matrial.priority = item.priority;
                            db.Entry(matrial).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                        {
                            Exception raise = dbEx;
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    string message = string.Format("{0}:{1}",
                                        validationErrors.Entry.Entity.ToString(),
                                        validationError.ErrorMessage);
                                    // raise a new exception nesting  
                                    // the current instance as InnerException  
                                    raise = new InvalidOperationException(message, raise);
                                }
                            }
                            throw raise;
                        }

                    }
                }
                
                foreach (var item in RapportSector.RequirementFitness)
                {
                    try {
                    var matrial = db.RequirementSector.Find(item.Id);
                    matrial.Id = item.Id;
                    matrial.IdSector = RapportSector.IdSector;
                    matrial.IdRequirement = item.IdRequirement;
                    matrial.percentage = item.percentage;
                    //matrial.priority = item.priority;
                    db.Entry(matrial).State = EntityState.Modified;
                    db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting  
                                // the current instance as InnerException  
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }

                }
                if (RapportSector.RequirementLeadershipVM != null)
                {
                    foreach (var item in RapportSector.RequirementLeadershipVM)
                    {
                        try
                        {
                            var matrial = db.LeadershipSector.Find(item.Id);
                            matrial.Id = item.Id;
                            matrial.IsChecked = item.IsChecked;
                            db.Entry(matrial).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                        {
                            Exception raise = dbEx;
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    string message = string.Format("{0}:{1}",
                                        validationErrors.Entry.Entity.ToString(),
                                        validationError.ErrorMessage);
                                    // raise a new exception nesting  
                                    // the current instance as InnerException  
                                    raise = new InvalidOperationException(message, raise);
                                }
                            }
                            throw raise;
                        }

                    }
                }

                TempData["UpdateSectorsuccess"] = " تم تعديل السلاح بنجاح!";
                return RedirectToAction("RaportsSector", "RequirementSectorVM", new RouteValueDictionary(new { id = RapportSector.IdSector }));
            }
            return View();
        }
    }
}