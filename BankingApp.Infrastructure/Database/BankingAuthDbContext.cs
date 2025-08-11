using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

    namespace BankingApp.Infrastructure.Database
    {
        public class BankingAuthDbContext : IdentityDbContext
        {
            public BankingAuthDbContext(DbContextOptions<BankingAuthDbContext> options) : base(options)

            {

            }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);

                var readerId = "5bc84df1-b43d-4830-b8b8-ccf750839ae2";
                var writerId = "d76b6975-c526-4dc1-af45-22c4bb248f54";

                var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerId,
                    ConcurrencyStamp = readerId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper(),
                },


                new IdentityRole
                {
                    Id = writerId,
                    ConcurrencyStamp = writerId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper(),
                }

            };

                builder.Entity<IdentityRole>()
                    .HasData(roles);
            }

        }
    }


