using FinancialAdviserAI.Core;
using FinancialAdviserAI.Core.Entities;
using FinancialAdviserAI.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinancialAdviserAI.Data.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly FinanceDataContext _context;

        public StockRepository(FinanceDataContext context)
        {
            _context = context;
        }

        public async Task<Stock> GetStockByTickerAsync(string ticker)
        {
            return await _context.Stocks.SingleOrDefaultAsync(s => s.Ticker == ticker);
        }

        public async Task AddStockAsync(Stock stock)
        {
            await _context.Stocks.AddAsync(stock);
            await _context.SaveChangesAsync();
        }
    }

}
