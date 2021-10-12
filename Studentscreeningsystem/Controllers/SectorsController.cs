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
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Studentscreeningsystem.Controllers
{
    public class SectorsController : Controller
    {
        private RBAC_Model db = new RBAC_Model();
     
        // GET: Sectors
        public ActionResult Index()
        {
          
            return View(db.Sector.ToList());
        }

        public ActionResult ExportToExcel()
        {
            var gv = new GridView();

            var query = from e in db.Sector
                        
                        select new { المسمى = e.NameSector, عددالشواغر=e.NbGraduates }; ;                     
                      
            gv.DataSource = query.ToList();
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ListSectorsExcel.xls");
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
            string strPDFFileName = string.Format("Sector" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
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

            float[] headers = { 50,50 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  

            List<Sector> Sectors = db.Sector.ToList();

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\Fonts\\simpfxo.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("قائمة الأسلحة", new Font(basefont, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            });


            ////Add header  

            AddCellToHeader(tableLayout, "عدد الشواغر");
            AddCellToHeader(tableLayout, "المسمى");
            

            ////Add body  

            foreach (var item in Sectors)
            {
                AddCellToBody(tableLayout, item.NbGraduates.ToString());
                AddCellToBody(tableLayout, item.NameSector);
         
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

        // GET: Sectors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sectors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdSector,NameSector,NbGraduates")] Sector sector, HttpPostedFileBase SectorsLogo)
        {
            
            if (ModelState.IsValid)
            {
                var results = db.Sector.Where(p => p.NameSector == sector.NameSector).ToList();
                

                
                if (results.Count > 0)
                {
                   

                    TempData["nameSectorerror"] = "عذراً ،مسمى السلاح أدخل من قبل في النظام!";
                    return RedirectToAction("Create", "Sectors");
                    
                }
                else
                {
                    if (SectorsLogo != null)
                    {
                        if (SectorsLogo.ContentType == "image/jpg" || SectorsLogo.ContentType == "image/png" || SectorsLogo.ContentType == "image/bmp" || SectorsLogo.ContentType == "image/gif" || SectorsLogo.ContentType == "image/jpeg")
                        {
                            SectorsLogo.SaveAs(Server.MapPath("/") + "/SectorsLogo/" + SectorsLogo.FileName);
                            sector.LogoPath = SectorsLogo.FileName;
                            db.Sector.Add(sector);
                            db.SaveChanges();
                            TempData["AddSectorsuccess"] = " تم إضافة مسمى السلاح بنجاح بالنظام!";
                            return RedirectToAction("Create", "RequirementSectorVM", new RouteValueDictionary(new { id = sector.IdSector }));
                        }
                        else {
                            TempData["logoerror"] = "عذراً ، الرجاء إدراج شعار !";
                            return View();
                        }
                            
                    }
                    else
                    {
                        TempData["logoerror"] = "عذراً ،لم يتم إدخال شعار السلاح في النظام!";
                        return View();
                    }
                        

                   
                }  
            }

            return View(sector);
        }

       

        // GET: Sectors/Delete/5
        public ActionResult Delete(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sector sector = db.Sector.Find(id);
            if (sector == null)
            {
                return HttpNotFound();
            }
            return View(sector);
        }

        // POST: Sectors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sector sector = db.Sector.Find(id);
            db.LeadershipSector.ToList().RemoveAll(m => m.IdSector == id);
            db.Specification.ToList().RemoveAll(m => m.IdSector == id);
            db.RequirementSector.ToList().RemoveAll(m => m.IdSector == id);
            db.Sector.Remove(sector);
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