using Studentscreeningsystem.ViewsModel;
using Studentscreeningsystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Diagnostics;
using System.Web.UI.WebControls;
using NonFactors.Mvc.Grid;
using System.IO;
using System.Web.UI;

namespace Studentscreeningsystem.Controllers
{
    [RBAC]
    public class GraduateArrangementVMController : Controller
    {
        private RBAC_Model db = new RBAC_Model();
       //تقوم بإرجاع قائمة الخريجين غير المستبعدين من عملية التوزيع والغير ملغين من النظام مرتبة حسب معدلاتهم ترتيباً تفاضلياً
        public List<GraduateArrangementVM> ListGraduateOrder()
        {
            
            var Graduate = db.SpecificationGraduate.Include(g => g.USER).Where(g => g.USER.Inactive != true && g.USER.Insortable != true).OrderByDescending(g => g.AverageGraduate).ToList();
            List<GraduateArrangementVM> listGraduateVM = new List<GraduateArrangementVM>();

            int count = 0;
            foreach (var item in Graduate)

            {
                count++;
                var Element = new GraduateArrangementVM
                {
                    IdGraduate = item.USER.User_Id,
                    NameGraduate = item.USER.Firstname + " " + item.USER.Lastname,
                    NumGraduate = Int64.Parse(item.USER.Passeword),
                    Average = item.AverageGraduate,
                    Rank = count
                };
                listGraduateVM.Add(Element);

            }
            return listGraduateVM;

        }

        public List<SortableGraduateVM> ListSortableGraduate()
        {

            //قائمة فارغة لترتيب الطلاب حسب السلاح الموجه له
            List<SortableGraduateVM> SortableGraduate = new List<SortableGraduateVM>();

            //قائمة الطلاب مرتبين حسب المعدل
            var ListGraduateVM = ListGraduateOrder().ToList();

            #region


            //قائمة الأسلحة
            var sectors = db.Sector.ToList();
            //يمثل مجموع عدد الشواغر 4% لكل الأسلحة
            double NbgDiv4 = 0;

            foreach (var sector in sectors)
            {
                //حساب مجموع 4% من الشواغر الأسلحة 
                NbgDiv4 += Math.Ceiling(sector.NbGraduates * 0.04);
            }
            Debug.WriteLine("------مجموع 4% من الشواغر السلاح " + NbgDiv4);
            //حساب عدد المجموعات 
            var nb = ListGraduateVM.Count / NbgDiv4;
            var Countnb = (int)Math.Ceiling(nb);
            
            Debug.WriteLine("------عدد المجموعات " + Countnb);

            for (int i = 0; i < Countnb; i++)
            {if(i< Countnb - 1) {
                List<SortableGraduateVM> SortableGraduateiteration = new List<SortableGraduateVM>();
                var list = ListGraduateVM.GetRange(index: i * (int)NbgDiv4, count: (int)NbgDiv4);
                Debug.WriteLine("******************list" + i + ": index=" + i * (int)NbgDiv4 + " count=" + (int)NbgDiv4 + "***********************************");
                SortableGraduate = Sortablelist(SortableGraduate, list, SortableGraduateiteration);
                Debug.WriteLine("*****************************************************************");
                Debug.WriteLine("");
                }
            else
                {
                    var x = ListGraduateVM.Count - ((Countnb - 1) * (int)NbgDiv4);
                    Debug.WriteLine("-------x= "+x);
                    List<SortableGraduateVM> SortableGraduateiteration = new List<SortableGraduateVM>();
                    var list = ListGraduateVM.GetRange(index: i * (int)NbgDiv4, count: x);
                    Debug.WriteLine("******************list" + i + ": index=" + i * (int)NbgDiv4 + " count=" + (int)NbgDiv4 + "***********************************");
                    SortableGraduate = Sortablelist(SortableGraduate, list, SortableGraduateiteration);
                    Debug.WriteLine("*****************************************************************");
                    Debug.WriteLine("");
                }
            }


            #endregion

            
            //إضافة الخريجين المستبعدين من الفرز و السلاح الموجه لهم يدويا
            var sg = db.SpecificationGraduate.Include(s => s.USER.Sector).Where(s => s.USER.Insortable == true && s.USER.IdSector != null).ToList();
            foreach (var item in sg)
            {
                var graduate = new SortableGraduateVM();
                graduate.IdGraduate = item.User_Id;
                graduate.NameGraduate = item.USER.Firstname + " " + item.USER.Lastname;
                graduate.NumGraduate = Int64.Parse(item.USER.Passeword);
                graduate.IdSector = item.USER.Sector.IdSector;
                graduate.NameSector = item.USER.Sector.NameSector;
                graduate.Average = item.AverageGraduate;
                SortableGraduate.Add(graduate);

            }

            return SortableGraduate;
        }

        // GET: GraduateArrangementVM .OrderByDescending(s => s.Average)
        public ActionResult Index()
        {

            var listGraduateVM = ListGraduateOrder();

            return View(listGraduateVM);
        }

        public ActionResult ExportToExcel()
        {

            List<SortableGraduateVM> SortableGraduate = ListSortableGraduate();
            var list = SortableGraduate.OrderByDescending(s => s.Average);
            int i = 1;
            foreach (var item in list)
            {
                item.Rank = i;
                i++;
            }
            var gv = new GridView();
            var query = list.Select(s => new
            {
                التسلسل=s.Rank,
                الرقم_العسكري = s.NumGraduate,
                اسم_الخريج = s.NameGraduate,
                المعدل = s.Average,
                السلاح_الموجه_له = s.NameSector
            });

            gv.DataSource = query.ToList();
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=قائمة توزيع الخريجين.xls");
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

            return RedirectToAction("SortableGraduate");
        }




        [HttpGet]
        public ActionResult SortableGraduate()
        {
            List<SortableGraduateVM> SortableGraduate = ListSortableGraduate();
            var list = SortableGraduate.OrderByDescending(s => s.Average);
            int i = 1;
            foreach (var item in list)
            {
                item.Rank = i;
                i++;
            }

            return View(list);
        }


        public List<SortableGraduateVM> Sortablelist(List<SortableGraduateVM> SortableGraduate, List<GraduateArrangementVM> ListGraduateVM, List<SortableGraduateVM> SortableGraduateiteration)
        {
            //قائمة الأسلحة
            var sectors = db.Sector.ToList();
            //يمثل مجموع عدد الشواغر 4% لكل الأسلحة
            double NbgDiv4 = 0;
            //يمثل مجموع عدد الشواغر لكل الأسلحة
            var SumNbg = 0;

            foreach (var sector in sectors)
            {
                SumNbg += sector.NbGraduates;

                NbgDiv4 += Math.Ceiling(sector.NbGraduates * 0.04);
            }

            //تمثل عدد تكرار عملية الفرز للوصول إلى مجموع عدد الشواغر
            var NBiteration = (int)Math.Ceiling(SumNbg / NbgDiv4);

            //List<SortableGraduateVM> SortableGraduateiteration = new List<SortableGraduateVM>();
            //قائمة للوضع الخريجين الذين درجاتهم لم تخول لهم التوجه الى الأسلحه في هذه الحلقة
            List<GraduateArrangementVM> ListGraduatePMin = new List<GraduateArrangementVM>();

            for (int i = 0; i < 25; i++)
            {
                //  دمج قائمة الخريجين الذين درجاتهم لم تخول لهم التوجه الى الأسلحه في  الحلقة السابقة الى  القائمة الطلاب لم يتم توجيههم
                ListGraduateVM = ListGraduateVM.Union(ListGraduatePMin).ToList();
                ListGraduatePMin.Clear();
                //    تعاد عملية الفرز مع حذف 5 نقاط من متطلبات الأسلحه اذا عدد الطلاب في القائمة الطلاب لم يتم توجيههم الى سلاح مخالف لصفر
                if (ListGraduateVM.Count != 0)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("حذف 5 * " + i + " من الدرجات المطلوبة بالنسبة للأسلحة");

                    for (int y = 0; y < NBiteration; y++)

                    {
                        Debug.WriteLine("-----------------------NBiteration :" + NBiteration +"---------------------------------");
                        //SortableGraduateiteration.Clear();

                        //S --------------------------------------------------------------------------------
                        //حذف الطلاب الذين تم فرزهم من القائمة
                        var ListGraduateVM2 = ListGraduateVM.ToArray();
                        foreach (var s in ListGraduateVM2)
                        {
                            foreach (var item in SortableGraduate)
                            {
                                if (s.IdGraduate == item.IdGraduate)
                                {
                                    ListGraduateVM.Remove(s);
                                }

                            }

                        }
                        //حذف الطلاب الذين درجاتهم لم تأهلهم لتوجه الى اي سلاح من القائمةلأعادة فرزهم عند حذف نقاط من درجات المطلوبة للأسلحه  
                        var ListGraduateVM3 = ListGraduateVM.ToArray();
                        foreach (var s in ListGraduateVM3)
                        {
                            foreach (var item in ListGraduatePMin)
                            {
                                if (s.IdGraduate == item.IdGraduate)
                                {
                                    ListGraduateVM.Remove(s);
                                }

                            }

                        }

                        //F --------------------------------------------------------------------------------F//



                        foreach (var Graduate in ListGraduateVM)

                        {
                          

                            // رغبات الطالب مرتبة
                            var listwishe = from t1 in db.GraduateWishes
                                            join t2 in db.Sector on t1.IdSector equals t2.IdSector
                                            where t1.User_Id.Equals(Graduate.IdGraduate)
                                            //نسحب الطالب حسب رغباته مرتبة rank : رغبة الطالب
                                            orderby t1.Rank
                                            select new {t1.IdSector,  t1.Rank,  t2.NameSector, idgraduate = t1.User_Id };
                            var listwishes = listwishe.ToList();
                            var x = 0;
                            Debug.WriteLine("===========================================");
                            Debug.WriteLine("اسم الخريج===>>> " + Graduate.NameGraduate);

                            //loop on listwishes of student order by rank  
                            foreach (var item in listwishes)
                            {

                                //عدد الطلاب الذين تم توجيهه الى هذا السلاح (رغبة الطالب
                                var nbgraduatesortablesector = SortableGraduate.FindAll(s => s.IdSector.Equals(item.IdSector)).Count();
                                //معلومات السلاح...
                                var Sector = db.Sector.Find(item.IdSector);
                                //عدد مقاعد  المطلوب لهذا السلاح
                                int nbgraduaterequie = Sector.NbGraduates;
                                Debug.WriteLine("السلاح===>>> " + item.NameSector + " ===عدد الخريجين الذين تم توجيههم الى هذا السلاح===>>> " + nbgraduatesortablesector + " ===عدد الشواغر المطلوب===>>> " + nbgraduaterequie);
                                //مقارنة عدد شواغر السلاح بعدد الطلاب الجملي الذين تم توجيهه الى هذا السلاح
                                if (nbgraduaterequie > nbgraduatesortablesector)
                                {

                                    //عدد الطلاب الذين تم توجيههم الى هذا السلاح في هذه الحلقة
                                    var nbgraduatesortablesectoriteration = SortableGraduateiteration.FindAll(s => s.IdSector.Equals(item.IdSector)).Count();

                                    //13------------------------------حساب قيمة 4 بالمائةمن عدد شواغر السلاح --------------------------------
                                    double value3 = nbgraduaterequie * (0.04 * (y + 1));
                                    var nb = (int)Math.Ceiling(value3);
                                    Debug.WriteLine("السلاح===>>> " + item.NameSector + " ===قيمة 4 بالمائة من عدد الشواغر ===>>> " + nb + " ===عدد الخريجين الذين تم توجيههم في هذه الحلقة ===>>> " + nbgraduatesortablesectoriteration);
                                    //13--------------------------------------------------------------

                                    //اذا عدد الطلاب الذين تم توجيههم في هذه الحلقةالى هذا السلاح لم يتجاؤز قيمة 4 بالمائة من عدد الشواغر لهذا السلاح
                                    if (nb > nbgraduatesortablesectoriteration)
                                    {
                                        ////توجييه الخريجين 10 الأوائل

                                        //if (Graduate.Rank >= 1 && Graduate.Rank <= 10)
                                        //{
                                        //    Debug.WriteLine("رتبة الخريج" + Graduate.Rank);
                                        //    var element = new SortableGraduateVM();
                                        //    element.IdSector = item.IdSector;
                                        //    element.NameSector = item.NameSector;
                                        //    element.IdGraduate = Graduate.IdGraduate;
                                        //    element.NumGraduate = Graduate.NumGraduate;
                                        //    element.NameGraduate = Graduate.NameGraduate;
                                        //    element.Average = Graduate.Average;
                                        //    SortableGraduate.Add(element);
                                        //    SortableGraduateiteration.Add(element);
                                        //    Debug.WriteLine("تم توجيه الخريج===>>> " + Graduate.NameGraduate + "الى السلاح===>>> " + item.NameSector + "---OK----");
                                        //    Debug.WriteLine("======================المرور الى الخريج الموالي=====================");
                                        //    break;

                                        //}


                                        // SELECT A.IdRequirement,A.percentage as pGraduate,B.percentage as psector
                                        //FROM DegreeGraduate As A
                                        //JOIN RequirementSector As B ON A.IdRequirement = B.IdRequirement
                                        // where A.User_Id = 10 and B.IdSector = 1 and A.percentage < B.percentage


                                        //مقارنة درجات الخريج بمتطلبات السلاح
                                        var Query = from DG in db.DegreeGraduate
                                                    join RS in db.RequirementSector on DG.IdRequirement equals RS.IdRequirement
                                                    where DG.User_Id == Graduate.IdGraduate && RS.IdSector == item.IdSector && RS.percentage - (5 * i) > 0 && DG.percentage < RS.percentage - (5 * i)
                                                    select DG;
                                        //اذا كانت درجات الخريج أكبر أو يساؤي لدرجات المطلوبة لسلاح يتم أضافته ألى قائمة الطلاب المؤجهين مع اسناد هذا السلاح له------------------------------------------------
                                        if (Query.ToList().Count == 0)
                                        {

                                            var element = new SortableGraduateVM
                                            {
                                                IdSector = item.IdSector,
                                                NameSector = item.NameSector,
                                                IdGraduate = Graduate.IdGraduate,
                                                NumGraduate = Graduate.NumGraduate,
                                                NameGraduate = Graduate.NameGraduate,
                                                Average = Graduate.Average
                                            };
                                            SortableGraduate.Add(element);
                                            SortableGraduateiteration.Add(element);
                                            //تسجيل السلاح الموجه للخريج في قاعدة البيانات
                                           
                                            USER user = db.USERS.Find(Graduate.IdGraduate);
                                            if (user != null)
                                            {                               
                                                db.Entry(user).Entity.IdSector = item.IdSector;
                                                db.Entry(user).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                            Debug.WriteLine("تم توجيه الخريج===>>> " + Graduate.NameGraduate + "الى السلاح===>>> " + item.NameSector + "---OK----");
                                            Debug.WriteLine("======================المرور الى الخريج الموالي=====================");
                                            break;

                                        }
                                        else
                                        {//اذا كان درجات الخريج لم تتوافق مع اي سلاح  يتم اضافته الى قائمة المستبعدين من الفرز ليتم اعادة فرزه عند حذف نقاط من درجات الأسلحه
                                            x++;
                                            if (x == listwishes.Count)
                                            {

                                                var element = new GraduateArrangementVM();
                                                {
                                                    element.IdGraduate = Graduate.IdGraduate;
                                                    element.NameGraduate = Graduate.NameGraduate;
                                                    element.NumGraduate = Graduate.NumGraduate;
                                                    element.Average = Graduate.Average;
                                                    element.Rank = Graduate.Rank;
                                                    ListGraduatePMin.Add(element);
                                                  
                                                }
                                                break;
                                            }
                                            Debug.WriteLine("درجات الطالب  ===>>> " + Graduate.NameGraduate + "لم تتوافق مع السلاح===>>> " + item.NameSector + "---Ko----");
                                            Debug.WriteLine("المرور الى الرغبة الموالية");
                                        }

                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("لم يعد هناك شواغر بنسبة لالسلاح  ===>>> " + item.NameSector + "---Ko----");
                                }
                            }
                        }


                    }
                }
                else
                {
                    break;
                   
                }

            }
            return SortableGraduate;
        }

        public ActionResult AfterSortableGraduate()
        {
         var GraduateList= db.SpecificationGraduate.Include(s => s.USER.Sector).Where(s=> s.USER.IdSector != null).OrderByDescending(s => s.AverageGraduate).ToList();
         List<SortableGraduateVM> SortableGraduate = new List<SortableGraduateVM>();
         var i = 0;
            foreach (var item in GraduateList)
            {
                i++;
                var Graduate = new SortableGraduateVM
                {
                    IdGraduate= item.User_Id,
                    NameGraduate=item.USER.Firstname +" "+ item.USER.Lastname,
                    NumGraduate= Int64.Parse(item.USER.Passeword),
                    IdSector= item.USER.IdSector,
                    NameSector=item.USER.Sector.NameSector,
                    Average=item.AverageGraduate,
                    Rank=i
                };
                SortableGraduate.Add(Graduate);
            }

            return View(SortableGraduate.ToList());
        }
    }
}