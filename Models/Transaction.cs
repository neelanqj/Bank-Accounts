using System;
using System.ComponentModel.DataAnnotations;

namespace Bank_Accounts.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User {get;set;}
    }
}