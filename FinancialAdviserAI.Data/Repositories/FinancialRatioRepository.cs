using FinancialAdviserAI.Core;
using FinancialAdviserAI.Core.Entities;
using FinancialAdviserAI.Core.Interfaces.Repositories;

namespace FinancialAdviserAI.Data.Repositories
{
    public class FinancialRatioRepository : IFinancialRatioRepository
    {
        private readonly FinanceDataContext _context;

        public FinancialRatioRepository(FinanceDataContext context)
        {
            _context = context;
        }

        public async Task AddFinancialRatioAsync(FinancialRatio financialRatio)
        {
            await _context.FinancialRatios.AddAsync(financialRatio);
            await _context.SaveChangesAsync();
        }
    }
}
