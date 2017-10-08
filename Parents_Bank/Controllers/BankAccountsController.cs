using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Parents_Bank.Models;

namespace Parents_Bank.Controllers
{
    [Authorize]
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult RedirectDetails(int? id)
        {
            return RedirectToAction("Details", "BankAccounts", new { id = id });
        }
        // GET: BankAccounts
        public ActionResult Index()
        {
            return View(db.BankAccounts.ToList());
        }

        public ActionResult NoAccess()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ViewBag.accId = db.BankAccounts.First(x => x.RecipientEmail == User.Identity.Name).Id;
            return View();
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount != null)
            {
                ViewBag.BalanceAmount = Math.Round(bankAccount.Transactions.Sum(x => x.Amount), 4);
                ViewBag.InterestAmount = Math.Round(bankAccount.InerestAmount() - ViewBag.BalanceAmount, 3);
            }
            
            
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public ActionResult Create()
        {
            bool accessCheck = db.BankAccounts.Any(x => x.RecipientEmail == User.Identity.Name);
            if (accessCheck)
                return RedirectToAction("NoAccess", "BankAccounts");

            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OwnerEmail,RecipientEmail,Name,OpenDate,InterestRate")] BankAccount bankAccount)
        {
            if(db.BankAccounts.Count(x=>x.OwnerEmail==bankAccount.RecipientEmail)>0)
                ModelState.AddModelError("RecipientEmail", "An owner cannot be a recipient of another account");
            if (db.BankAccounts.Count(x => x.RecipientEmail == bankAccount.OwnerEmail) > 0)
                ModelState.AddModelError("OwnerEmail", "A recipient cannot be an owner of another account");
            if (db.BankAccounts.Count(x => x.RecipientEmail == bankAccount.RecipientEmail) > 0)
                ModelState.AddModelError("RecipientEmail", "A recipient user can only have 1 recipient account");
            if (ModelState.IsValid)
            {
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            bool accessCheck = db.BankAccounts.Any(x => x.RecipientEmail == User.Identity.Name);
            if (accessCheck)
                return RedirectToAction("NoAccess", "BankAccounts");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerEmail,RecipientEmail,Name,OpenDate,InterestRate")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            bool accessCheck = db.BankAccounts.Any(x => x.RecipientEmail == User.Identity.Name);
            if (accessCheck)
                return RedirectToAction("NoAccess", "BankAccounts");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount account= db.BankAccounts.First(x => x.Id == id);
            if (account.IsOwner(User.Identity.Name))
            {
                account.AccountBalance = account.Balance();
                if (account.AccountBalance > 0 || account.AccountBalance < 0)
                {
                    ModelState.AddModelError("", "An account cannot be deleted if there is a balance on the account");
                    return RedirectToAction("Index");
                }


                BankAccount bankAccount = db.BankAccounts.Find(id);
                db.BankAccounts.Remove(bankAccount);
                db.SaveChanges();
            }
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
