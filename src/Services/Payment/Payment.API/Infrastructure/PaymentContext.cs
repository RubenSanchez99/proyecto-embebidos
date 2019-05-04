namespace Payment.API.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Microsoft.EntityFrameworkCore.Design;

    public class PaymentContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {
        }
        public DbSet<PaymentAccount> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.ForNpgsqlUseIdentityColumns();
        }     
    }
}