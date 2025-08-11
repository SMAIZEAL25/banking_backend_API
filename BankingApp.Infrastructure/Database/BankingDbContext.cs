using BankingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Add this using directive

namespace BankingApp.Infrastructure.Persistence
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER CONFIG
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .Ignore(u => u.FullName); // Computed property, no DB column

            modelBuilder.Entity<User>()
                .HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ACCOUNT CONFIG
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.AccountNumber)
                .IsUnique();
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email);

            modelBuilder.Entity<Account>()
                .Property(a => a.CurrentBalance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Account>()
                .Property(a => a.AccountStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Account>()
                .Property(a => a.Currency)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Account>()
                .Property(a => a.AccountType)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Transactions linked to Account
            modelBuilder.Entity<Account>()
                .HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // TRANSACTION CONFIG
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => new { t.UserId, t.AccountId });
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.TransactionDate);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.CurrentBalance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.TransactionType)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.TransactionStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.BeneficiaryBank)
                .HasMaxLength(50);
        }
    }
}