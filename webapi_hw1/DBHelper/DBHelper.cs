using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webapi_hw1.DBHelper
{
    public class AccountType
    {
        [Key]
        public int ID { get; set; }
        public string AccountDescription { get; set; }

        // Navigation properties.
        // Child.        
        public virtual ICollection<ClientAccount>
            ClientAccounts
        { get; set; }
    }

    public class ClientProfile
    {
        [Key]
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        // Navigation properties.
        // Child.
        public virtual ICollection<ClientAccount>
            ClientAccounts
        { get; set; }
    }

    public class ClientAccount
    {
        [Key, Column(Order = 0)]
        public int ClientID { get; set; }
        [Key, Column(Order = 0)]
        public int AccountID { get; set; }
        public int Balance { get; set; }

        // Navigation properties.
        // Parents.
        public virtual ClientProfile ClientProfile { get; set; }
        public virtual AccountType AccountType { get; set; }
    }

    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options)
            : base(options) {}

        // Define entity collections.
        public DbSet<ClientProfile> ClientProiles { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<ClientAccount> ClientAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //---------------------------------------------------------------
            // Define composite primary keys.
            modelBuilder.Entity<ClientAccount>()
                .HasKey(ca => new { ca.ClientID, ca.AccountID });

            //---------------------------------------------------------------
            // Define foreign keys here. Do not use foreign key annotations.
            modelBuilder.Entity<ClientAccount>()
                .HasOne(ca => ca.ClientProfile) // Parent
                .WithMany(cp => cp.ClientAccounts) // Child
                .HasForeignKey(fk => new { fk.ClientID })
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<ClientAccount>()
                .HasOne(ca => ca.AccountType) // Parent
                .WithMany(at => at.ClientAccounts) // Child
                .HasForeignKey(fk => new { fk.AccountID })
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            Seed(modelBuilder);
        }

        void Seed(ModelBuilder builder)
        {
            // Seed parents first and then children since child FK's point to parents.
            builder.Entity<ClientProfile>().HasData(
                new { ID = 1, LastName = "Chu", FirstName = "Tina" },
                new { ID = 2, LastName = "Gateman", FirstName = "Max" }
            );
            builder.Entity<AccountType>().HasData(
                new { ID = 1, AccountDescription = "Admin" },
                new {ID = 2, AccountDescription = "User" }
            );
            builder.Entity<ClientAccount>().HasData(
                new { ClientID = 1, AccountID = 2, Balance = 100 },
                new { ClientID = 2, AccountID = 1, Balance = 900 }
            );
            // https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding
        }
    }
}
