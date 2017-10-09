using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Parents_Bank.Models;

namespace Parents_Bank.Controllers
{
    [Authorize]
    public class WishListItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public static int _bankAccount;

        public ActionResult RedirectDetails(int? id)
        {
            return RedirectToAction("Details", "BankAccounts", new { id = id });
        }
        // GET: WishListItems
        public ActionResult Index()
        {
            var wishListItems = db.WishListItems.Include(t => t.Account)
                .Where(t => t.Account.OwnerEmail == User.Identity.Name);
            
            return View(wishListItems.ToList());
        }

        public ActionResult AccountDetailsWishListItems(int id)
        {
            _bankAccount = id;
            var bankAccount = db.BankAccounts.Find(id);
            var wishListItems = db.WishListItems.Include(t => t.Account)
                .Where(t => t.Account.RecipientEmail == bankAccount.RecipientEmail);
            
            return View(wishListItems.ToList());
        }

        // GET: WishListItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishListItem wishListItem = db.WishListItems.Find(id);
            if (wishListItem != null)
            {
                BankAccount account = db.BankAccounts.First(x => x.Id == wishListItem.AccountId);
                var currentUser = User.Identity.Name;
                if (account.IsOwnerOrRecipient(currentUser))
                {
                    return View(wishListItem);
                } 
            }
            return HttpNotFound();
        }

        // GET: WishListItems/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = _bankAccount;
            return View();
        }

        // POST: WishListItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,WebAddress,PurchasedTag")] WishListItem wishListItem)
        {
            if (ModelState.IsValid)
            {
                BankAccount account = db.BankAccounts.Find(_bankAccount);
                wishListItem.Account = account;
                wishListItem.AccountId = account.Id;
                db.WishListItems.Add(wishListItem);
                db.SaveChanges();
                return RedirectToAction("Details","BankAccounts", new { @id = db.BankAccounts.First(x => x.Id == wishListItem.AccountId).Id });
            }

            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", wishListItem.AccountId);
            return View(wishListItem);
        }

        // GET: WishListItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishListItem wishListItem = db.WishListItems.Find(id);
            BankAccount account = db.BankAccounts.First(x => x.Id == wishListItem.AccountId);

            var currentUser = User.Identity.Name;
            if (account.IsOwnerOrRecipient(User.Identity.Name))
            {
             
                if (wishListItem == null)
                {
                    return HttpNotFound();
                }
                ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", wishListItem.AccountId);
                return View(wishListItem);
            }
            return HttpNotFound();
        }

        // POST: WishListItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,WebAddress,PurchasedTag")] WishListItem wishListItem)
        {
            if (ModelState.IsValid)
            {
                var updateWishList = db.WishListItems.Find(wishListItem.Id);
                updateWishList.DateAdded = wishListItem.DateAdded;
                updateWishList.Cost = wishListItem.Cost;
                updateWishList.Description = wishListItem.Description;
                updateWishList.WebAddress = wishListItem.WebAddress;
                updateWishList.PurchasedTag = wishListItem.PurchasedTag;
                db.Entry(updateWishList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RedirectDetails",new{id= updateWishList.AccountId});
            }
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", wishListItem.AccountId);
            return View(wishListItem);
        }

        // GET: WishListItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishListItem wishListItem = db.WishListItems.Find(id);
            BankAccount account = db.BankAccounts.First(x => x.Id == wishListItem.AccountId);
            var currentUser = User.Identity.Name;
            if (account.IsOwnerOrRecipient(currentUser))
            {
                if (wishListItem == null)
                {
                    return HttpNotFound();
                }
                return View(wishListItem);
            }
            return HttpNotFound();
        }

        // POST: WishListItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WishListItem wishListItem = db.WishListItems.Find(id);
            if (wishListItem != null && wishListItem.Account.IsOwnerOrRecipient(User.Identity.Name))
            {
                db.WishListItems.Remove(wishListItem);
                db.SaveChanges();
                return RedirectToAction("RedirectDetails",new {id=wishListItem.AccountId});
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
