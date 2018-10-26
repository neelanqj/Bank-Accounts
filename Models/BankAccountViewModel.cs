using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bank_Accounts.Models
{
    public class BankAccountViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        [DataType(DataType.Currency)]
        public decimal CurrentBalance { get; set; }
        [DataType(DataType.Currency)]
        public decimal? DepositOrWithdrawalAmount { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}