using System.Collections.Generic;
using DbUpgrade.Models;
using Microsoft.EntityFrameworkCore;

namespace DbUpgrade.EF
{
    public class DbUpContext : DbContext
    {
        private readonly DbUpConfiguration _settings;
        public DbUpContext(DbContextOptions<DbUpContext> options, DbUpConfiguration settings)
            : base(options)
        {
            _settings = settings;
        }
        
        public DbSet<DbVersion> Versions{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Common");
            modelBuilder.Entity<DbVersion>()
                .Property(b => b.Id)
                .UseIdentityColumn()
                .IsRequired();

            modelBuilder.Entity<DbVersion>()
                .Property(b => b.DatabasePrefix)
                .IsRequired();

            modelBuilder.Entity<DbVersion>()
                .Property(b => b.Module)
                .IsRequired();

            modelBuilder.Entity<DbVersion>()
                .Property(b => b.Version)
                .IsRequired();

            modelBuilder.Entity<DbVersion>()
                .Property(b => b.IsActive)
                .IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_settings.ConnectionString);
            }
        }

    }
}
