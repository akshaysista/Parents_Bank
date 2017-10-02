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

        // GET: BankAccounts
        public async Task<ActionResult> Index()
        {
            return View(await db.BankAccounts.ToListAsync());
        }

        // GET: BankAccounts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = await db.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,OwnerEmail,RecipientEmail,Name,OpenDate,InterestRate")] BankAccount bankAccount)
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
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = await db.BankAccounts.FindAsync(id);
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,OwnerEmail,RecipientEmail,Name,OpenDate,InterestRate")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = await db.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            List<Transaction> transactions = db.BankAccounts.First(x => x.Id == id).Transactions;
            decimal balance = 0;
            foreach (var item in transactions)
            {
                balance += item.Amount;
            }
            if (balance <= 0)
            {
                ModelState.AddModelError("","An account cannot be deleted if there is a balance on the account");
                return RedirectToAction("Index");
            }
            else
            {
                BankAccount bankAccount = await db.BankAccounts.FindAsync(id);
                db.BankAccounts.Remove(bankAccount);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
