using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Studentscreeningsystem.Models;

namespace Studentscreeningsystem.Controllers
{
    public class MouvementUsersController : Controller
    {
        private RBAC_Model db = new RBAC_Model();

        // GET: MouvementUsers
        public ActionResult Index()
        {
            var mouvementUsers = db.MouvementUsers.Include(m => m.USER);
            return View(mouvementUsers.ToList());
        }

        [HttpGet]
        public PartialViewResult Search4Mouvements(DateTime _StartDate, DateTime _FinDate)
        {
            return PartialView("_ListMovementUsersTable", GetFilteredUserList(_StartDate, _FinDate));
        }
        [HttpGet]
        public PartialViewResult filterReset()
        {
            return PartialView("_ListMovementUsersTable", db.MouvementUsers.Include(m => m.USER).ToList());
        }
        private IEnumerable<MouvementUsers> GetFilteredUserList(DateTime _StartDate, DateTime _FinDate)
        {
            IEnumerable<MouvementUsers> _ret = null;
            try
            {
                if (_StartDate == DateTime.MinValue || _FinDate == DateTime.MinValue)
                {
                    _ret = db.MouvementUsers.Include(m => m.USER).ToList();
                }
                else
                {
                    _ret = db.MouvementUsers.Include(m => m.USER).Where(d => d.DateMouvement >= _StartDate && d.DateMouvement <= _FinDate).ToList();
                }
            }
            catch
            {
            }
            return _ret;
        }

        public PartialViewResult DeleteMouvements(DateTime _StartDate, DateTime _FinDate)
        {
            IList<MouvementUsers> mouvementUsers = GetFilteredUserList(_StartDate, _FinDate).ToList();
            foreach (var item in mouvementUsers)
            {
                MouvementUsers mouvementUser = db.MouvementUsers.Find(item.Id);
                db.MouvementUsers.Remove(mouvementUser);
                db.SaveChanges();
            }

            return PartialView("_ListMovementUsersTable", db.MouvementUsers.Include(m => m.USER).ToList());
        }

        public PartialViewResult DeleteAllMouvements()
        {
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE MouvementUsers");

            return PartialView("_ListMovementUsersTable", db.MouvementUsers.Include(m => m.USER).ToList());
        }
        // GET: MouvementUsers/Create
        public void Create(int userid, String link)
        {
            var MouvementUsers = new MouvementUsers();
            MouvementUsers.User_Id = userid;
            MouvementUsers.Link = link;
            MouvementUsers.DateMouvement = System.DateTime.Now;
            db.MouvementUsers.Add(MouvementUsers);
            db.SaveChanges();



        }



        // GET: MouvementUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MouvementUsers mouvementUsers = db.MouvementUsers.Find(id);
            if (mouvementUsers == null)
            {
                return HttpNotFound();
            }
            return View(mouvementUsers);
        }

        // POST: MouvementUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MouvementUsers mouvementUsers = db.MouvementUsers.Find(id);
            db.MouvementUsers.Remove(mouvementUsers);
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

