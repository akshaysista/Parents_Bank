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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult RedirectDetails(int? id)
        {
            return RedirectToAction("Details", "BankAccounts", new { id = id });
        }
        public ActionResult NoAccess()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ViewBag.accId = db.BankAccounts.First(x => x.RecipientEmail == User.Identity.Name).Id;

            return View();
        }
        public ActionResult AccountsList()
        {
            var transactions = db.Transactions.Include(t => t.Account)
                .Where(t=>t.Account.OwnerEmail==User.Identity.Name||t.Account.RecipientEmail==User.Identity.Name);
            return View(transactions.ToList());
        }
        // GET: Transactions

        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Account)
                .Where(t => t.Account.OwnerEmail == User.Identity.Name || t.Account.RecipientEmail == User.Identity.Name);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            BankAccount account = db.BankAccounts.First(x => x.Id == transaction.AccountId);
            var currentUser = User.Identity.Name;
            if (account.IsOwnerOrRecipient(currentUser))
            {
                if (transaction == null)
                {
                    return HttpNotFound();
                }
                return View(transaction);
            }
            return HttpNotFound();
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            bool accessCheck = db.BankAccounts.Any(x => x.RecipientEmail == User.Identity.Name);
            if (accessCheck)
                return RedirectToAction("NoAccess", "Transactions");

            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,TransactionDate,Amount,Note")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                BankAccount account = db.BankAccounts.First(x => x.OwnerEmail == User.Identity.Name);
                transaction.AccountId = account.Id;
                transaction.Account = account;
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Details","BankAccounts", new { @id = db.BankAccounts.First(x => x.Id == transaction.AccountId).Id });
            }

            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            bool accessCheck = db.BankAccounts.Any(x => x.RecipientEmail == User.Identity.Name);
            if (accessCheck)
                return RedirectToAction("NoAccess", "Transactions");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            BankAccount account = db.BankAccounts.First(x => x.Id == transaction.AccountId);
            var currentUser = User.Identity.Name;
            if (account.IsOwnerOrRecipient(currentUser))
            {
                if (transaction == null)
                {
                    return HttpNotFound();
                }
                ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", transaction.AccountId);
                return View(transaction);
            }
            return HttpNotFound();
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,TransactionDate,Amount,Note")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RedirectDetails",new {id=transaction.AccountId});
            }
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            bool accessCheck = db.BankAccounts.Any(x => x.RecipientEmail == User.Identity.Name);
            if (accessCheck)
                return RedirectToAction("NoAccess", "Transactions");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            BankAccount account = db.BankAccounts.First(x => x.Id == transaction.AccountId);
            var currentUser = User.Identity.Name;
            if (account.IsOwnerOrRecipient(currentUser))
            {
                if (transaction == null)
                {
                    return HttpNotFound();
                }
                return View(transaction);
            }
            return HttpNotFound();
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            Transaction transaction = db.Transactions.Find(id);
            if (transaction != null && transaction.Account.IsOwner(User.Identity.Name))
            {
                db.Transactions.Remove(transaction);
                db.SaveChanges();
                return RedirectToAction("RedirectDetails",new{id=transaction.AccountId});
            }
            else
            {
                return HttpNotFound();
            }
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
