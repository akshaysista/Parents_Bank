using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parents_Bank.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Parents_Bank.Models.Transaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<Parents_Bank.Models.BankAccount> BankAccounts { get; set; }

        public System.Data.Entity.DbSet<Parents_Bank.Models.WishListItem> WishListItems { get; set; }
    }
}