using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parents_Bank.Models
{
    [CustomValidation(typeof(BankAccount),"ValidateInterestRate")]
    [CustomValidation(typeof(BankAccount),"ValidateEmailAddresses")]
    public class BankAccount
    {
        public int Id { get; set; }
        [EmailAddress]
        public string  OwnerEmail { get; set; }
        [EmailAddress]
        public string RecipientEmail { get; set; }
        [Required]
        public string Name { get; set; }//Name of child
        public DateTime OpenDate { get; set; }
        public decimal Balance { get; set; }
        
        public decimal InterestRate { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
        public virtual List<WishListItem> WishListItems { get; set; }

        public static ValidationResult ValidateInterestRate(BankAccount bankAccount, ValidationContext context)
        {
            if (bankAccount ==null)
            {
                return ValidationResult.Success;
            }
            if (bankAccount.InterestRate <=0)
            {
                return new ValidationResult("Interest rate cannot be 0% or below");
            }
            if (bankAccount.InterestRate > 100)
            {
                return new ValidationResult("Interest rate cannot be 100% or above");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateEmailAddresses(BankAccount bankAccount ,ValidationContext context)
        {
            if (bankAccount==null)
            {
                return ValidationResult.Success;
            }
            if (string.Equals(bankAccount.OwnerEmail,bankAccount.RecipientEmail,StringComparison.InvariantCultureIgnoreCase))
            {
                return new ValidationResult("Owner and Recipient Cannot Have Same Email Address");
            }
            return ValidationResult.Success;
        }
        public decimal YeartoDateInterestEarned()
        {
            return 0;
        }
    }
}