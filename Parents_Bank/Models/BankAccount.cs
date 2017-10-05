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
        [EmailAddress(ErrorMessage = "Owner field must be a valid email")]
        [Display(Name = "Owner Email")]
        public string  OwnerEmail { get; set; }
        [Display(Name = "Recipient Email")]
        [EmailAddress(ErrorMessage = "Recipient field must be a valid email")]
        public string RecipientEmail { get; set; }
        [Required]
        [Display(Name = "Name of Recipient")]
        public string Name { get; set; }//Name of child
        [Display(Name = "Account Open Date")]
        public DateTime OpenDate { get; set; }
        [Display(Name = "Rate of Interest")]
        public decimal InterestRate { get; set; }
        [Display(Name = "Account Balance")]
        public decimal  AccountBalance { set; get; }
        public virtual List<Transaction> Transactions { get; set; }
        public virtual List<WishListItem> WishListItems { get; set; }

        public decimal Balance()
        {
            AccountBalance = 0;
            foreach (var item in Transactions)
            {
                AccountBalance += item.Amount;
            }
            return AccountBalance;
        }
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
        public bool IsOwner(string currentUser)
        {
            // HELPER METHOD TO CHECK IF THE USER PASSED IN AS THE ARGUMENT
            // IS THE OWNER
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return false;
            }

            if (currentUser.ToLower() == OwnerEmail.ToLower())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsRecipient(string currentUser)
        {
            // HELPER METHOD TO CHECK IF THE USER PASSED IN AS THE ARGUMENT
            // IS THE RECIPIENT
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return false;
            }

            if (currentUser.ToLower() == RecipientEmail.ToLower())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsOwnerOrRecipient(string currentUser)
        {
            // HELPER METHOD TO CHECK IF THE USER PASSED IN AS THE ARGUMENT
            // IS THE OWNER OR THE RECIPIENT
            return IsOwner(currentUser) || IsRecipient(currentUser);
        }
    }
}