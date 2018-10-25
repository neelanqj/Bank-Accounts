using Bank_Accounts.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_Accounts.Persistence
{
    public class BankAccountDbContext: DbContext
    {
        public DbSet<User> Users { get; set; } 
        public DbSet<Transaction> Transactions { get; set; }
        public BankAccountDbContext(DbContextOptions<BankAccountDbContext> options)
            : base(options)
        {
            
        }
        
    }
}