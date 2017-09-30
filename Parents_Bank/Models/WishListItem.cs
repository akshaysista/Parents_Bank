using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parents_Bank.Models
{
    public class WishListItem
    {
        public int Id { get; set; }
        public virtual int AccountId { get; set; }
        public virtual BankAccount Account { get; set; }
        public DateTime DateAdded { get; set; }
        [Required]
        public double Cost { get; set; }
        [Required]
        public string Description { get; set; }
        [Url(ErrorMessage = "Web address must be valid url")]
        public string WebAddress { get; set; }
        public bool PurchasedTag { get; set; }
    }
}