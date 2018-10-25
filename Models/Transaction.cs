using System;

namespace Bank_Accounts.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
    }
}