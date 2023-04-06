using FinancialAdviserAI.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialAdviserAI.Core
{
    public class FinanceDataContext : DbContext
    {
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<FinancialNews> FinancialNews { get; set; }
        public DbSet<FinancialStatement> FinancialStatements { get; set; }
        public DbSet<FinancialRatio> FinancialRatios { get; set; }

        public FinanceDataContext(DbContextOptions<FinanceDataContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>()
                .HasMany(s => s.FinancialStatements)
                .WithOne(fs => fs.Stock)
                .HasForeignKey(fs => fs.StockId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Stock>()
                .HasMany(s => s.FinancialRatios)
                .WithOne(fr => fr.Stock)
                .HasForeignKey(fr => fr.StockId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
