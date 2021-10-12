using iTextSharp.text;
using iTextSharp.text.pdf;
using Studentscreeningsystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Studentscreeningsystem.Controllers
{
    public class RequirementsController : Controller
    {
        private RBAC_Model db = new RBAC_Model();

        // GET: Requirements
        public ActionResult Index()
        {
            return View(db.Requirement.ToList());
        }

        public ActionResult ExportToExcel()
        {
            var gv = new GridView();
            var query = from e in db.Requirement
                        select new
                        {
                            المتطلب = e.NameRequirement,

                            الصنف = e.Categorie.ToString()
                        };
            gv.DataSource = query.ToList();
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=RequirementExcel.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");

        }


        // GET: Requirements/Create
        public ActionResult Create()
        {
            return View();
        }

              [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdRequirement,NameRequirement,Categorie")] Requirement requirement)
        {
            if (ModelState.IsValid)
            {
                db.Requirement.Add(requirement);
                db.SaveChanges();
                var sector=db.Sector.ToList();
                foreach (var item in sector)
                {
                    var RequirementSector = new RequirementSector
                    {
                        IdSector = item.IdSector, IdRequirement = requirement.IdRequirement, percentage = 0
                    };
                    //RequirementSector.priority = 1;
                    db.RequirementSector.Add(RequirementSector);
                    db.SaveChanges();

                }
                var graduate = db.USERS.Where(g => g.Title == "خريج").ToList();
                foreach (var item in graduate)
                {
                    var DegreeGraduate = new DegreeGraduate
                    {
                        User_Id = item.User_Id, IdRequirement = requirement.IdRequirement, percentage = 0
                    };

                    db.DegreeGraduate.Add(DegreeGraduate);
                    db.SaveChanges();

                }


                return RedirectToAction("Index");
            }

            return View(requirement);
        }

        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created  
            string strPDFFileName = string.Format("Requirement" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();

            //Create PDF Table with 2 columns  
            PdfPTable tableLayout = new PdfPTable(2);

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

            float[] headers = { 50, 50 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  

            List<Requirement> Requirements = db.Requirement.ToList();

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\Fonts\\simpfxo.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("قائمة متطلبات الأسلحة", new Font(basefont, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            });


            ////Add header  
            
            AddCellToHeader(tableLayout, "الصنف");
            AddCellToHeader(tableLayout, "المتطلب");


            ////Add body  

            foreach (var emp in Requirements)
            {

               
                AddCellToBody(tableLayout, emp.Categorie.ToString());
                AddCellToBody(tableLayout, emp.NameRequirement);


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

        // GET: Requirements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requirement requirement = db.Requirement.Find(id);
            if (requirement == null)
            {
                return HttpNotFound();
            }
            return View(requirement);
        }

        // POST: Requirements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdRequirement,NameRequirement,Categorie")] Requirement requirement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requirement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(requirement);
        }

        // GET: Requirements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requirement requirement = db.Requirement.Find(id);
            if (requirement == null)
            {
                return HttpNotFound();
            }
            return View(requirement);
        }

        // POST: Requirements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Requirement requirement = db.Requirement.Find(id);
            db.Requirement.Remove(requirement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
