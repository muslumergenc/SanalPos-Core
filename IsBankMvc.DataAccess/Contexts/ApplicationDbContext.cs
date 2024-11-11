using IsBankMvc.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IsBankMvc.DataAccess.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PaymentCard> PaymentCards { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
