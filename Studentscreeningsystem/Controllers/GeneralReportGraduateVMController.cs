using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Studentscreeningsystem.ViewsModel;
using Studentscreeningsystem.Models;

namespace Studentscreeningsystem.Controllers
{
    public class GeneralReportGraduateVMController : Controller
    {
        private RBAC_Model db = new RBAC_Model();
        // GET: GeneralReportGraduateVM
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ListGraduate", "Admin");
            }
            var Graduate = db.USERS.Find(id);


            if (Graduate == null || Graduate.Title != "خريج")
            {
                return RedirectToAction("ListGraduate", "Admin");
            }
            else
            {
                var Spesification = db.SpecificationGraduate.Where(m => m.User_Id == id).SingleOrDefault();
                var Wishes = from T1 in db.GraduateWishes
                             join T2 in db.Sector
                             on T1.IdSector equals T2.IdSector
                             where T1.User_Id == id
                             orderby T1.Rank
                             select new {T1.IdSector, T1.Rank, T2.NameSector };

                var Matrial = from T1 in db.DegreeGraduate
                              join T2 in db.RequirementSector
                              on T1.IdRequirement equals T2.IdRequirement
                              join T3 in db.Requirement
                              on T2.IdRequirement equals T3.IdRequirement
                              where T1.User_Id == id && (int)T3.Categorie == 0
                              select new { PercentageGraduate = T1.percentage, PercentageSector = T2.percentage,T2.IdSector, T3.NameRequirement };

                var Fitness = from T1 in db.DegreeGraduate
                              join T2 in db.RequirementSector
                              on T1.IdRequirement equals T2.IdRequirement
                              join T3 in db.Requirement
                              on T2.IdRequirement equals T3.IdRequirement
                              where T1.User_Id == id && (int)T3.Categorie == 1
                              select new { PercentageGraduate = T1.percentage, PercentageSector = T2.percentage,  T2.IdSector,T3.NameRequirement };

                var Mesure = from T1 in db.DegreeGraduate
                             join T2 in db.RequirementSector
                             on T1.IdRequirement equals T2.IdRequirement
                             join T3 in db.Requirement
                             on T2.IdRequirement equals T3.IdRequirement
                             where T1.User_Id == id && (int)T3.Categorie == 2
                             select new { PercentageGraduate = T1.percentage, PercentageSector = T2.percentage,  T2.IdSector, T3.NameRequirement };

                var Behavor = from T1 in db.DegreeGraduate
                              join T2 in db.RequirementSector
                              on T1.IdRequirement equals T2.IdRequirement
                              join T3 in db.Requirement
                              on T2.IdRequirement equals T3.IdRequirement
                              where T1.User_Id == id && (int)T3.Categorie == 3
                              select new { PercentageGraduate = T1.percentage, PercentageSector = T2.percentage,  T2.IdSector, T3.NameRequirement };
                var Leadership = from T1 in db.LeadershipGraduate
                                 join T2 in db.LeadershipSector
                                 on T1.IdLeadership equals T2.IdLeadership
                                 join T3 in db.Leadershiprequirement
                                 on T2.IdLeadership equals T3.IdLeadership
                                 where T1.User_Id == id
                                 select new { IsCheckedGraduate = T1.IsChecked, IsCheckedSector = T2.IsChecked,  T2.IdSector,T3.NameLeadership };

                List<WishesVM> listWishesVM = new List<WishesVM>();
                List<PercentageGSVM> listMaterialVM = new List<PercentageGSVM>();
                List<PercentageGSVM> listMesureVM = new List<PercentageGSVM>();
                List<PercentageGSVM> listFitnessVM = new List<PercentageGSVM>();
                List<PercentageGSVM> listBehavorVM = new List<PercentageGSVM>();
                List<LeadershipGSVM> listLeadershipVM = new List<LeadershipGSVM>();

                foreach (var item in Wishes)

                {
                    var DetailsWishesVM = new WishesVM
                    {
                        IdWishes = item.IdSector, NameWishes = item.NameSector, Rank = item.Rank
                    };
                    listWishesVM.Add(DetailsWishesVM);

                }
                for (int i = 0; i < listWishesVM.Count; i++)
                {
                    foreach (var item in Matrial)

                    {
                        if (item.IdSector == listWishesVM[i].IdWishes)
                        {
                            var DetailsMatrialVM = new PercentageGSVM
                            {
                                IdSector = item.IdSector,
                                NameRequirement = item.NameRequirement,
                                PercentageGraduate = item.PercentageGraduate,
                                PercentageSector = item.PercentageSector
                            };
                            listMaterialVM.Add(DetailsMatrialVM);

                        }

                    }





                    foreach (var item in Fitness)

                    {
                        if (item.IdSector == listWishesVM[i].IdWishes)
                        {
                            var DetailsFitnessVM = new PercentageGSVM
                            {
                                IdSector = item.IdSector,
                                NameRequirement = item.NameRequirement,
                                PercentageGraduate = item.PercentageGraduate,
                                PercentageSector = item.PercentageSector
                            };
                            listFitnessVM.Add(DetailsFitnessVM);

                        }

                    }

                    foreach (var item in Mesure)

                    {
                        if (item.IdSector == listWishesVM[i].IdWishes)
                        {
                            var DetailsMesureVM = new PercentageGSVM
                            {
                                IdSector = item.IdSector,
                                NameRequirement = item.NameRequirement,
                                PercentageGraduate = item.PercentageGraduate,
                                PercentageSector = item.PercentageSector
                            };
                            listMesureVM.Add(DetailsMesureVM);
                        }

                    }

                    foreach (var item in Behavor)

                    {
                        if (item.IdSector == listWishesVM[i].IdWishes)
                        {
                            var DetailsBehavorVM = new PercentageGSVM
                            {
                                IdSector = item.IdSector,
                                NameRequirement = item.NameRequirement,
                                PercentageGraduate = item.PercentageGraduate,
                                PercentageSector = item.PercentageSector
                            };
                            listBehavorVM.Add(DetailsBehavorVM);
                        }

                    }

                    foreach (var item in Leadership)

                    {
                        if (item.IdSector == listWishesVM[i].IdWishes)
                        {
                            var DetailsLeadershipVM = new LeadershipGSVM
                            {
                                IdSector = item.IdSector,
                                IsCheckedGraduate = item.IsCheckedGraduate,
                                IsCheckedSector = item.IsCheckedSector,
                                NameLeadership = item.NameLeadership
                            };

                            listLeadershipVM.Add(DetailsLeadershipVM);
                        }

                    }
                }





                var RapportGeneralGraduate = new GeneralReportGraduateVM
                {
                    IdGraduate = Graduate.User_Id,
                    NumGraduate = Int64.Parse(Graduate.Passeword),
                    IdentityGraduate = Int64.Parse(Graduate.Username),
                    NameGraduate = Graduate.Firstname + " " + Graduate.Lastname,
                    AverageGraduate = Spesification.AverageGraduate,
                    WishesList = listWishesVM,
                    MaterialList = listMaterialVM,
                    FitnessList = listFitnessVM,
                    MeasureList = listMesureVM,
                    BehaviorList = listBehavorVM,
                    LeadershipList = listLeadershipVM
                };


                return View(RapportGeneralGraduate);

            }


        }
    }
}