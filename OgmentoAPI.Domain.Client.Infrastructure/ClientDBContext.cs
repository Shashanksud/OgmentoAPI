﻿using Microsoft.EntityFrameworkCore;
using OgmentoAPI.Domain.Client.Abstractions.DataContext;

namespace OgmentoAPI.Domain.Client.Infrastructure
{
    public class ClientDBContext: DbContext
    {
        public ClientDBContext(DbContextOptions<ClientDBContext>options): base(options) { }
        public DbSet<SalesCenter> SalesCenters { get; set; }
        public DbSet<SalesCenterUserMapping> SalesCentreUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SalesCenter>()
                .HasOne(sc => sc.Country)
                .WithMany()
                .HasForeignKey(sc => sc.CountryId);
            

            modelBuilder.Entity<SalesCenterUserMapping>()
                .HasKey(sc => new { sc.UserID, sc.SalesCentreID });

            modelBuilder.Entity<SalesCenterUserMapping>()
                .HasOne(sc => sc.SalesCenters)
                .WithMany(sc => sc.SalesCenterUsers)
                .HasForeignKey(sc => sc.SalesCentreID)
                .HasForeignKey(sc => sc.UserID);

            base.OnModelCreating(modelBuilder);
        }

    }
}
