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
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace Studentscreeningsystem.Controllers
{
    public class alldegreeGraduateController : Controller
    {
        private RBAC_Model db = new RBAC_Model();
      
        public ActionResult ExportToExcel()
        {
            var gv = new GridView();
            var query = 
                        from e in db.USERS
                        where e.Title == "خريج" && e.Inactive != true
                        orderby e.Lastname
                       
                        select new
                        {
                           الاسم = e.Firstname + " " + e.Lastname,
                            الرقمالعسكري = e.Passeword,
                    رقم = e.Username,

                        };
            gv.DataSource = query.ToList();
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=GraduateExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");



        }
        public FileResult CreatePdf()
        {
            
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created  
            string strPDFFileName = string.Format("ListGraduate" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();

            //Create PDF Table with 2 columns  
            PdfPTable tableLayout = new PdfPTable(3);

            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF  
            doc.Add(Add_Content_To_PDF(tableLayout));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {

            List <float> headers = new List<float>{30,30,30  }; //Header Widths 
           
            tableLayout.SetWidths(headers.ToArray()); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  

            var users = db.USERS.Where(r => r.Title == "خريج" && (r.Inactive == false || r.Inactive == null)).OrderBy(r => r.Firstname).ThenBy(r => r.Lastname).ToList();
            var requirement = db.Requirement.GroupBy(g => g.Categorie).ToList();

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\Fonts\\simpfxo.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("قائمة الخرجين", new Font(basefont, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            });


            ////Add header  


            AddCellToHeader(tableLayout, "الاسم");
            AddCellToHeader(tableLayout, "الرقم العسكري");
            AddCellToHeader(tableLayout, "رقم الهوية");
            


            ////Add body  

            foreach (var item in users)
            {

                AddCellToBody(tableLayout, item.Firstname+""+item.Lastname);
                AddCellToBody(tableLayout, item.Passeword);
                AddCellToBody(tableLayout, item.Username);
               

            }

            return tableLayout;
        }
        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\Fonts\\simpfxo.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(basefont, 8, 1, BaseColor.YELLOW)))
            {
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)

        {
            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\Fonts\\simpfxo.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(basefont, 8, 1, BaseColor.BLACK)))
            {
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }


        public ActionResult Create(int Id)

        {
            var Spesification = db.SpecificationGraduate.Where(m => m.User_Id == Id).SingleOrDefault();
            IList<DegreeGraduate> requirment = db.DegreeGraduate.Where(a => a.User_Id == Id).ToList();
            IList<LeadershipGraduate> Leadership = db.LeadershipGraduate.Where(a => a.User_Id == Id).ToList();

            IList<Requirement> RequirementMaterial = db.Requirement.Where(a => (int)a.Categorie == 0).ToList();
            IList<Requirement> Requirementsuplement = db.Requirement.Where(a => (int)a.Categorie == 3).ToList();
            IList<Requirement> RequirementFitness = db.Requirement.Where(a => (int)a.Categorie == 1).ToList();
            IList<Requirement> Requirementmesure = db.Requirement.Where(a => (int)a.Categorie == 2).ToList();
            IList<Leadershiprequirement> RequirementLeadership = db.Leadershiprequirement.ToList();
            if (Spesification != null )
            {
                TempData["adddagreeSuccess"] = " تم تسجيل درجات الخريج مسبقاً!";
                return RedirectToAction("EditGraduate", "alldegreeGraduate", new RouteValueDictionary(new { id = Id }));
            }
            ViewBag.RequirementMaterial = RequirementMaterial;
            ViewBag.Requirementsuplement = Requirementsuplement;
            ViewBag.RequirementFitness = RequirementFitness;
            ViewBag.Requirementmesure = Requirementmesure;
            ViewBag.RequirementLeadership = RequirementLeadership;
            //ViewBag.IdGraduate = new SelectList(db.Graduate, "IdGraduate", "NumGraduate");
            ViewBag.IdGraduate = Id;
            ViewBag.Graduate = db.USERS.Find(Id);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(alldegreeGraduateVM alldegreeGraduateVM)
        {
            if (ModelState.IsValid)
            {
                var Specification = new SpecificationGraduate();
                Specification.User_Id = alldegreeGraduateVM.IdGraduate;
                Specification.Length = alldegreeGraduateVM.lenght;
                Specification.Weight = alldegreeGraduateVM.lenght;
                Specification.AverageGraduate = alldegreeGraduateVM.AverageGraduate;
                db.SpecificationGraduate.Add(Specification);
                db.SaveChanges();

                foreach (var item in alldegreeGraduateVM.degreesupplement)
                {
                    var DegreeGraduate = new DegreeGraduate();
                    DegreeGraduate.User_Id = alldegreeGraduateVM.IdGraduate;
                    DegreeGraduate.IdRequirement = item.IdRequirement;
                    DegreeGraduate.percentage = item.percentage;


                    db.DegreeGraduate.Add(DegreeGraduate);
                    db.SaveChanges();

                }

                foreach (var item in alldegreeGraduateVM.degreeMatrial)
                {
                    var DegreeGraduate = new DegreeGraduate();
                    DegreeGraduate.User_Id = alldegreeGraduateVM.IdGraduate;
                    DegreeGraduate.IdRequirement = item.IdRequirement;
                    DegreeGraduate.percentage = item.percentage;


                    db.DegreeGraduate.Add(DegreeGraduate);
                    db.SaveChanges();

                }


                foreach (var item in alldegreeGraduateVM.degreeFitness)
                {
                    var DegreeGraduate = new DegreeGraduate();
                    DegreeGraduate.User_Id = alldegreeGraduateVM.IdGraduate;
                    DegreeGraduate.IdRequirement = item.IdRequirement;
                    DegreeGraduate.percentage = item.percentage;


                    db.DegreeGraduate.Add(DegreeGraduate);
                    db.SaveChanges();

                }

                foreach (var item in alldegreeGraduateVM.degreeMesure)
                {
                    var DegreeGraduate = new DegreeGraduate();
                    DegreeGraduate.User_Id = alldegreeGraduateVM.IdGraduate;
                    DegreeGraduate.IdRequirement = item.IdRequirement;
                    DegreeGraduate.percentage = item.percentage;


                    db.DegreeGraduate.Add(DegreeGraduate);
                    db.SaveChanges();

                }

                foreach (var item in alldegreeGraduateVM.LeadershipVM)
                {
                    var LeadershipGraduate = new LeadershipGraduate();
                    LeadershipGraduate.User_Id = alldegreeGraduateVM.IdGraduate;
                    LeadershipGraduate.IdLeadership = item.IdLeadership;
                    LeadershipGraduate.IsChecked = item.IsChecked;


                    db.LeadershipGraduate.Add(LeadershipGraduate);
                    db.SaveChanges();

                }
                db.SaveChanges();
                return RedirectToAction("RaportsGraduate", "alldegreeGraduate", new RouteValueDictionary(new { id = alldegreeGraduateVM.IdGraduate }));
            }

           // IList<SpecificationGraduate> spesification = db.SpecificationGraduate.Where(a => a.User_Id == alldegreeGraduateVM.IdGraduate).ToList();
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
            
            return View(alldegreeGraduateVM);
        }

        public ActionResult RaportsGraduate(int? id)
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

                var RequirementMaterial = db.DegreeGraduate.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 0);
                var listMaterial = RequirementMaterial.Where(m => m.User_Id == id).ToList();

                var Requirementsuplement = db.DegreeGraduate.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 3);
                var listsuplement = Requirementsuplement.Where(m => m.User_Id == id).ToList();

                var RequirementFitness = db.DegreeGraduate.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 1);
                var listFitness = RequirementFitness.Where(m => m.User_Id == id).ToList();

                var Requirementmesure = db.DegreeGraduate.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 2);
                var listmesure = Requirementmesure.Where(m => m.User_Id == id).ToList();

                var RequirementLeadership = db.LeadershipGraduate.Include(m => m.Leadershiprequirement);
                var listLeadership = RequirementLeadership.Where(m => m.User_Id == id).ToList();

                if (Spesification == null)
                {
                    TempData["notadddegree"] = "لم يتم تسجيل درجات الخريج!";
                    return RedirectToAction("Create", "alldegreeGraduate", new RouteValueDictionary(new { id = Graduate.User_Id }));
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
                        var DetailsRequirementVM = new DetailsRequirementVM();
                        DetailsRequirementVM.NameRequirement = item.Requirement.NameRequirement;
                        DetailsRequirementVM.percentage = item.percentage;

                        listMaterialVM.Add(DetailsRequirementVM);

                    }

                    foreach (var item in listsuplement)

                    {
                        var DetailsRequirementVM = new DetailsRequirementVM();
                        DetailsRequirementVM.NameRequirement = item.Requirement.NameRequirement;
                        DetailsRequirementVM.percentage = item.percentage;

                        listsuplementVM.Add(DetailsRequirementVM);

                    }

                    foreach (var item in listFitness)

                    {
                        var DetailsRequirementVM = new DetailsRequirementVM();
                        DetailsRequirementVM.NameRequirement = item.Requirement.NameRequirement;
                        DetailsRequirementVM.percentage = item.percentage;

                        listFitnessVM.Add(DetailsRequirementVM);

                    }

                    foreach (var item in listmesure)

                    {
                        var DetailsRequirementVM = new DetailsRequirementVM();
                        DetailsRequirementVM.NameRequirement = item.Requirement.NameRequirement;
                        DetailsRequirementVM.percentage = item.percentage;

                        listmesureVM.Add(DetailsRequirementVM);

                    }

                    foreach (var item in listLeadership)

                    {
                        var DetailsLeadershipVM = new DetailsLeadershipVM();
                        DetailsLeadershipVM.NameLeadership = item.Leadershiprequirement.NameLeadership;
                        DetailsLeadershipVM.IsChecked = item.IsChecked;

                        listLeadershipVM.Add(DetailsLeadershipVM);

                    }

                    var RapportGraduate = new alldegreeGraduateVM
                    {
                        IdGraduate = Graduate.User_Id,
                        Firstname = Graduate.Firstname,
                        Lastname = Graduate.Lastname,
                        NumGraduate = Int64.Parse(Graduate.Passeword),
                        IdentityGraduate = Int64.Parse(Graduate.Username),
                        lenght = Spesification.Length,
                        weight = Spesification.Weight,
                        AverageGraduate= Spesification.AverageGraduate,
                        LeadershipVM = listLeadershipVM,
                        degreeMatrial = listMaterialVM,
                        degreesupplement = listsuplementVM,
                        degreeFitness = listFitnessVM,
                        degreeMesure = listmesureVM

                    };


                    return View(RapportGraduate);
                }
            }

        }

        public ActionResult EditGraduate(int? id)
        {
            if (id == null )
            {
                return RedirectToAction("ListGraduate","Admin");
            }
            var Graduate = db.USERS.Find(id);
            if (Graduate == null|| Graduate.Title!= "خريج")
            {
                return RedirectToAction("ListGraduate", "Admin");
            }
            else {
                var Spesification = db.SpecificationGraduate.Where(m => m.User_Id == id).SingleOrDefault();

                var RequirementMaterial = db.DegreeGraduate.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 0);
                var listMaterial = RequirementMaterial.Where(m => m.User_Id == id).ToList();

                var Requirementsuplement = db.DegreeGraduate.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 3);
                var listsuplement = Requirementsuplement.Where(m => m.User_Id == id).ToList();

                var RequirementFitness = db.DegreeGraduate.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 1);
                var listFitness = RequirementFitness.Where(m => m.User_Id == id).ToList();

                var Requirementmesure = db.DegreeGraduate.Include(m => m.Requirement).Where(m => (int)m.Requirement.Categorie == 2);
                var listmesure = Requirementmesure.Where(m => m.User_Id == id).ToList();

                var RequirementLeadership = db.LeadershipGraduate.Include(m => m.Leadershiprequirement);
                var listLeadership = RequirementLeadership.Where(m => m.User_Id == id).ToList();

                    if (Spesification == null )
                    {
                        TempData["notFinichaddDegree"] = "عذراً ، لم تكمل إدخال درجات الخريج !";
                        return RedirectToAction("Create", "alldegreeGraduate", new RouteValueDictionary(new { id = Graduate.User_Id}));
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
                            var DetailsRequirementVM = new DetailsRequirementVM();
                            DetailsRequirementVM.Id = item.Id;
                            DetailsRequirementVM.IdRequirement = item.Requirement.IdRequirement;
                            DetailsRequirementVM.NameRequirement = item.Requirement.NameRequirement;
                            DetailsRequirementVM.percentage = item.percentage;

                            listMaterialVM.Add(DetailsRequirementVM);

                        }

                        foreach (var item in listsuplement)

                        {
                            var DetailsRequirementVM = new DetailsRequirementVM();
                            DetailsRequirementVM.Id = item.Id;
                            DetailsRequirementVM.IdRequirement = item.Requirement.IdRequirement;
                            DetailsRequirementVM.NameRequirement = item.Requirement.NameRequirement;
                            DetailsRequirementVM.percentage = item.percentage;

                            listsuplementVM.Add(DetailsRequirementVM);

                        }

                        foreach (var item in listFitness)

                        {
                            var DetailsRequirementVM = new DetailsRequirementVM();
                            DetailsRequirementVM.Id = item.Id;
                            DetailsRequirementVM.IdRequirement = item.Requirement.IdRequirement;
                            DetailsRequirementVM.NameRequirement = item.Requirement.NameRequirement;
                            DetailsRequirementVM.percentage = item.percentage;

                            listFitnessVM.Add(DetailsRequirementVM);

                        }

                        foreach (var item in listmesure)

                        {
                            var DetailsRequirementVM = new DetailsRequirementVM();
                            DetailsRequirementVM.Id = item.Id;
                            DetailsRequirementVM.IdRequirement = item.Requirement.IdRequirement;
                            DetailsRequirementVM.NameRequirement = item.Requirement.NameRequirement;
                            DetailsRequirementVM.percentage = item.percentage;

                            listmesureVM.Add(DetailsRequirementVM);

                        }

                        foreach (var item in listLeadership)

                        {
                            var DetailsLeadershipVM = new DetailsLeadershipVM();
                            DetailsLeadershipVM.Id = item.Id;
                            DetailsLeadershipVM.IdLeadership = item.Leadershiprequirement.IdLeadership;
                            DetailsLeadershipVM.NameLeadership = item.Leadershiprequirement.NameLeadership;
                            DetailsLeadershipVM.IsChecked = item.IsChecked;

                            listLeadershipVM.Add(DetailsLeadershipVM);

                        }

                        var RapportGraduate = new alldegreeGraduateVM
                        {
                            IdGraduate = Graduate.User_Id,
                            Firstname = Graduate.Firstname,
                            Lastname = Graduate.Lastname ,
                            NumGraduate = Int64.Parse(Graduate.Passeword),
                            IdentityGraduate = Int64.Parse(Graduate.Username),
                            lenght = Spesification.Length,
                            weight = Spesification.Weight,
                            AverageGraduate= Spesification.AverageGraduate,
                            LeadershipVM = listLeadershipVM,
                            degreeMatrial = listMaterialVM,
                            degreesupplement = listsuplementVM,
                            degreeFitness = listFitnessVM,
                            degreeMesure = listmesureVM

                        };
                        ViewBag.Graduate = db.USERS.Find(id);
                        return View(RapportGraduate);
                     }

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGraduate(alldegreeGraduateVM RapportGraduate)
        {
            if (ModelState.IsValid)
            {
                var Specification = db.SpecificationGraduate.Where(m => m.User_Id == RapportGraduate.IdGraduate).SingleOrDefault();
                Specification.Length = RapportGraduate.lenght;
                Specification.Weight= RapportGraduate.weight;
                Specification.AverageGraduate = RapportGraduate.AverageGraduate;
                db.Entry(Specification).State = EntityState.Modified;
                db.SaveChanges();

               

                foreach (var item in RapportGraduate.degreeMatrial)
                {
                    try
                    {
                        DegreeGraduate DegreeGraduate = db.DegreeGraduate.Find(item.Id);
                        DegreeGraduate.Id = item.Id;
                        DegreeGraduate.User_Id = RapportGraduate.IdGraduate;
                        DegreeGraduate.IdRequirement = item.IdRequirement;
                        DegreeGraduate.percentage = item.percentage;
                        db.Entry(DegreeGraduate).State = EntityState.Modified;
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
                foreach (var item in RapportGraduate.degreeMesure)
                {
                    try
                    {
                        DegreeGraduate DegreeGraduate = db.DegreeGraduate.Find(item.Id);
                        DegreeGraduate.Id = item.Id;
                        DegreeGraduate.User_Id = RapportGraduate.IdGraduate;
                        DegreeGraduate.IdRequirement = item.IdRequirement;
                        DegreeGraduate.percentage = item.percentage;
                        db.Entry(DegreeGraduate).State = EntityState.Modified;
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
                if (RapportGraduate.degreesupplement != null)
                {
                    foreach (var item in RapportGraduate.degreesupplement)
                    {
                        try
                        {
                            DegreeGraduate DegreeGraduate = db.DegreeGraduate.Find(item.Id);
                            DegreeGraduate.Id = item.Id;
                            DegreeGraduate.User_Id = RapportGraduate.IdGraduate;
                            DegreeGraduate.IdRequirement = item.IdRequirement;
                            DegreeGraduate.percentage = item.percentage;
                            db.Entry(DegreeGraduate).State = EntityState.Modified;
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

                foreach (var item in RapportGraduate.degreeFitness)
                {
                    try
                    {
                        DegreeGraduate DegreeGraduate = db.DegreeGraduate.Find(item.Id);
                        DegreeGraduate.Id = item.Id;
                        DegreeGraduate.User_Id = RapportGraduate.IdGraduate;
                        DegreeGraduate.IdRequirement = item.IdRequirement;
                        DegreeGraduate.percentage = item.percentage;
                        db.Entry(DegreeGraduate).State = EntityState.Modified;
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
                if (RapportGraduate.LeadershipVM != null)
                {
                    foreach (var item in RapportGraduate.LeadershipVM)
                    {
                        try
                        {
                            LeadershipGraduate Leadership = db.LeadershipGraduate.Find(item.Id);
                            Leadership.Id = item.Id;
                            Leadership.User_Id = RapportGraduate.IdGraduate;
                            Leadership.IsChecked = item.IsChecked;
                            db.Entry(Leadership).State = EntityState.Modified;
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

                TempData["updateDegreeSuccess"] = " تم تعديل درجات الخريج بنجاح!";
                return RedirectToAction("RaportsGraduate", "alldegreeGraduate", new RouteValueDictionary(new { id = RapportGraduate.IdGraduate}));
            }
            return View();
        }

        public ActionResult GetDegreeGraduateExcel()
        {
            ViewBag.Requirments = db.Requirement.ToList();
            return View();
        }

        public ActionResult save_DegreeGraduate_excel(int idrequirement,HttpPostedFileBase file)
        {
            var message = "";
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                file = files[0];
                string fname;
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                }

                var Pathfile = Path.Combine(Server.MapPath("~/ExcelDegreeGraduates/"), fname);
                var save = true;
                var i = 0;
                while (save)
                {
                    if (System.IO.File.Exists(Pathfile))
                    {
                        string filename = Path.GetFileNameWithoutExtension(Pathfile);

                        filename = filename + "-" + i++;
                        string extention = Path.GetExtension(Pathfile);
                        filename = filename + extention;
                        Pathfile = Path.Combine(Server.MapPath("~/ExcelDegreeGraduates/"), filename);
                    }
                    else
                    {
                        save = false;
                    }
                }
                file.SaveAs(Pathfile);

                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + Pathfile + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";
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

                var List = new List<DegreeGraduate>();
                var Listgradutenotvalid = new List<String>();
                
               
                while (dReader.Read())
                {
                    String cin = dReader[0].ToString();
                    String percentage = dReader[1].ToString();
                    var idGraduate = db.USERS.Where(u => u.Username == cin && u.Title=="خريج").Select(u=>u.User_Id).SingleOrDefault();
                    if (idGraduate != 0)
                    {
                        
                        var degreeGraduate = new DegreeGraduate()
                        {
                            IdRequirement = idrequirement,
                            User_Id = idGraduate,
                            percentage = Int32.Parse(percentage),
                        };
                        List.Add(degreeGraduate);

                    }
                    else
                    {
                       
                        Listgradutenotvalid.Add(cin);
                    }


                }
                foreach (var item in List)
                {
                    var existdegree = db.DegreeGraduate.Where(d => d.IdRequirement == item.IdRequirement && d.User_Id == item.User_Id).SingleOrDefault();
                    if (existdegree != null)
                    {
                        existdegree.percentage = item.percentage;
                        db.Entry(existdegree).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var add = new DegreeGraduate()
                        {
                            IdRequirement = item.IdRequirement,
                            User_Id = item.User_Id,
                            percentage = item.percentage
                        };
                        db.DegreeGraduate.Add(add);
                        db.SaveChanges();
                    }

                    Debug.WriteLine("User_Id:   " + item.User_Id+ "   IdRequirement:   " +item.IdRequirement+ "  percentage:  "+item.percentage  );
                }
                
                excelConnection.Close();
                if(Listgradutenotvalid ==null || Listgradutenotvalid.Count() == 0)
                {
                    message = "تم إضافة درجات الخريجين في  ملف Excel بنجاح!";
                    TempData["SuccessuploadedDegExcel"] = message;
                }
                else
                {
                    var listcin = "";
                    foreach (var item in Listgradutenotvalid)
                    {
                        listcin = listcin+" " + item+" , ";
                    };
                    message = "هناك بعض المدخلات لم يتم إضافتها لعدم تطابق رقم الخريج مع أي خريج بالنظام(" + listcin + ")";
                    TempData["warninguploadedDegExcel"] = message;
                }
                

                

               
            }
            else
            {
                message = "لم تتم إضافة المستخدمين من ملف الإكسل !";
                TempData["DangeruploadedDegExcel"] = message;

            }
            Debug.WriteLine("message" + message);
            return Json(message);
        }
       public ActionResult AddAverageFromExcel()
        {
            return View();
        }
        public ActionResult SaveAddAverageFromExcel(HttpPostedFileBase file)
        {
            var message = "";
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                file = files[0];
                string fname;
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                }

                var Pathfile = Path.Combine(Server.MapPath("~/ExcelDegreeGraduates/"), fname);
                var save = true;
                var i = 0;
                while (save)
                {
                    if (System.IO.File.Exists(Pathfile))
                    {
                        string filename = Path.GetFileNameWithoutExtension(Pathfile);

                        filename = filename + "-" + i++;
                        string extention = Path.GetExtension(Pathfile);
                        filename = filename + extention;
                        Pathfile = Path.Combine(Server.MapPath("~/ExcelDegreeGraduates/"), filename);
                    }
                    else
                    {
                        save = false;
                    }
                }
                file.SaveAs(Pathfile);

                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + Pathfile + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";
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

                var List = new List<DegreeGraduate>();
                var Listgradutenotvalid = new List<String>();
                while (dReader.Read())
                {
                    String cin = dReader[0].ToString();
                    String percentage = dReader[1].ToString();
                    var idGraduate = db.USERS.Where(u => u.Username == cin && u.Title == "خريج").Select(u => u.User_Id).SingleOrDefault();
                    if (idGraduate != 0)
                    {
                        var spec = db.SpecificationGraduate.FirstOrDefault(x=>x.User_Id == idGraduate);
                        if (spec != null)
                        {
                            spec.AverageGraduate = float.Parse(percentage);
                        }

                        db.SaveChanges();

                    }
                    else
                    {

                        Listgradutenotvalid.Add(cin);
                    }
                }
                excelConnection.Close();
                if (Listgradutenotvalid == null || Listgradutenotvalid.Count() == 0)
                {
                    message = "تم إضافة درجات الخريجين في  ملف Excel بنجاح!";
                    TempData["SuccessuploadedDegExcel"] = message;
                }
                else
                {
                    var listcin = "";
                    foreach (var item in Listgradutenotvalid)
                    {
                        listcin = listcin + " " + item + " , ";
                    };
                    message = "هناك بعض المدخلات لم يتم إضافتها لعدم تطابق رقم الخريج مع أي خريج بالنظام(" + listcin + ")";
                    TempData["warninguploadedDegExcel"] = message;
                }
            }
            else
            {
                message = "لم تتم إضافة المستخدمين من ملف الإكسل !";
                TempData["DangeruploadedDegExcel"] = message;
            }
            Debug.WriteLine("message" + message);
            return Json(message);
        }
    }
}