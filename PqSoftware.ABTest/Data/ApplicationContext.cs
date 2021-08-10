using Microsoft.EntityFrameworkCore;
using PqSoftware.ABTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<LifetimeCount> LifetimeCounts { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<LifetimeCount>().HasNoKey();
        }
    }
}
