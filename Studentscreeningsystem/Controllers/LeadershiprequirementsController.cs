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
    public class LeadershiprequirementsController : Controller
    {
        private RBAC_Model db = new RBAC_Model();

        // GET: Leadershiprequirements
        public ActionResult Index()
        {
            return View(db.Leadershiprequirement.ToList());
        }

        public ActionResult ExportToExcel()
        {
            var gv = new GridView();
            var query = from e in db.Leadershiprequirement
                        select new
                        {
                            المتطلب = e.NameLeadership,

                        };
            gv.DataSource = query.ToList();
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=LeadershipExcel.xls");
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
            string strPDFFileName = string.Format("Leadership" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();

            //Create PDF Table with 2 columns  
            PdfPTable tableLayout = new PdfPTable(1);

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

            float[] headers = { 50 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  

            List<Leadershiprequirement> Leadershiprequirements = db.Leadershiprequirement.ToList();

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\Fonts\\simpfxo.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("قائمة متطلبات القياديه للأسلحة", new Font(basefont, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            });


            ////Add header  

          
            AddCellToHeader(tableLayout, "المتطلب");


            ////Add body  

            foreach (var item in Leadershiprequirements)
            {

                AddCellToBody(tableLayout,item.NameLeadership);

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

        // GET: Leadershiprequirements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leadershiprequirement leadershiprequirement = db.Leadershiprequirement.Find(id);
            if (leadershiprequirement == null)
            {
                return HttpNotFound();
            }
            return View(leadershiprequirement);
        }

        // GET: Leadershiprequirements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Leadershiprequirements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdLeadership,NameLeadership")] Leadershiprequirement leadershiprequirement)
        {
            if (ModelState.IsValid)
            {
                db.Leadershiprequirement.Add(leadershiprequirement);
                db.SaveChanges();
                var sector = db.Sector.ToList();
                foreach (var item in sector)
                {
                    var LeaderShipSector = new LeadershipSector();
                    LeaderShipSector.IdSector = item.IdSector;
                    LeaderShipSector.IdLeadership = leadershiprequirement.IdLeadership;
                    LeaderShipSector.IsChecked = false;
                    
                    db.LeadershipSector.Add(LeaderShipSector);
                    db.SaveChanges();

                }
                var graduate = db.USERS.Where(g => g.Title == "خريج").ToList();
                foreach (var item in graduate)
                {
                    var LeaderShipGraduate = new LeadershipGraduate();
                    LeaderShipGraduate.User_Id = item.User_Id;
                    LeaderShipGraduate.IdLeadership = leadershiprequirement.IdLeadership;
                    LeaderShipGraduate.IsChecked = false;

                    db.LeadershipGraduate.Add(LeaderShipGraduate);
                    db.SaveChanges();

                }
                return RedirectToAction("Index");
            }

            return View(leadershiprequirement);
        }

        // GET: Leadershiprequirements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leadershiprequirement leadershiprequirement = db.Leadershiprequirement.Find(id);
            if (leadershiprequirement == null)
            {
                return HttpNotFound();
            }
            return View(leadershiprequirement);
        }

        // POST: Leadershiprequirements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdLeadership,NameLeadership")] Leadershiprequirement leadershiprequirement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(leadershiprequirement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(leadershiprequirement);
        }

        // GET: Leadershiprequirements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leadershiprequirement leadershiprequirement = db.Leadershiprequirement.Find(id);
            if (leadershiprequirement == null)
            {
                return HttpNotFound();
            }
            return View(leadershiprequirement);
        }

        // POST: Leadershiprequirements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Leadershiprequirement leadershiprequirement = db.Leadershiprequirement.Find(id);
            db.Leadershiprequirement.Remove(leadershiprequirement);
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
