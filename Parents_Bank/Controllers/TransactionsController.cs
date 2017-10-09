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
        public static int _bankAccount;
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
        public ActionResult AccountsList(int id, bool isOwner)
        {
            _bankAccount = id;
            ViewBag.IsOwner = isOwner;
            var transactions = db.Transactions.Where(t => t.AccountId == id).OrderByDescending(t=>t.TransactionDate);
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
            if (transaction!=null)
            {
                BankAccount account = db.BankAccounts.First(x => x.Id == transaction.AccountId);
                var currentUser = User.Identity.Name;
                if (account.IsOwnerOrRecipient(currentUser))
                {
                    
                    return View(transaction);
                } 
            }
            return HttpNotFound();
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            bool accessCheck = db.BankAccounts.Any(x => x.RecipientEmail == User.Identity.Name);
            if (accessCheck)
                return RedirectToAction("NoAccess", "Transactions");

            ViewBag.AccountId = _bankAccount;
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
                BankAccount account = db.BankAccounts.Find(_bankAccount);
                var acctBalance = db.Transactions.Where(x => x.AccountId == account.Id).Sum(x => x.Amount);
                if ((transaction.Amount+acctBalance)<0)
                {
                   ModelState.AddModelError("Amount", "A Debit Cannot Be For More Than The Current Account Balance");
                    return View(transaction);
                }
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
                var updatedtransaction = db.Transactions.Find(transaction.Id);
                updatedtransaction.TransactionDate = transaction.TransactionDate;
                updatedtransaction.Amount=transaction.Amount;
                updatedtransaction.Note=transaction.Note;
                db.Entry(updatedtransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RedirectDetails",new {id= updatedtransaction.AccountId});
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
