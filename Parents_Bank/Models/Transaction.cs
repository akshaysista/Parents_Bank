﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Parents_Bank.Models
{
    [CustomValidation(typeof(Transaction), "ValidateTransactionAmount")]
    [CustomValidation(typeof(Transaction), "ValidateTransactionDate")]
    public class Transaction
    {
        public int Id { get; set; }
        public virtual int AccountId { get; set; }
        public virtual BankAccount Account { get; set; }
        public DateTime TransactionDate { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Note { get; set; }

        public static ValidationResult ValidateTransactionAmount(Transaction transaction, BankAccount bankAccount, ValidationContext context)
        {
            if(transaction==null||bankAccount==null)
                return  ValidationResult.Success;
            if(transaction.Amount>bankAccount.Balance)
                return  new ValidationResult("A debit cannot be for more that the current account balance");
            if (transaction.Amount>0)
                return ValidationResult.Success;
           else
           {
               return new ValidationResult("A transaction cannot be for a $0.00 amount");
           }
        }

        public static ValidationResult ValidateTransactionDate(Transaction transaction, ValidationContext context)
        {
            if(transaction.TransactionDate>DateTime.Now)
                return new ValidationResult("The transaction date cannot be in the future");
            if(transaction.TransactionDate.Year<DateTime.Now.Year)
                return new ValidationResult("The transaction date cannot be before the current year");
            return ValidationResult.Success;
        }



    }
}