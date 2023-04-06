using FinancialAdviserAI.Core;
using FinancialAdviserAI.Core.Entities;
using FinancialAdviserAI.Core.Interfaces.Repositories;

namespace FinancialAdviserAI.Data.Repositories
{
    public class FinancialStatementRepository : IFinancialStatementRepository
    {
        private readonly FinanceDataContext _context;

        public FinancialStatementRepository(FinanceDataContext context)

        {
            _context = context;
        }

        public async Task AddFinancialStatementAsync(FinancialStatement financialStatement)
        {
            await _context.FinancialStatements.AddAsync(financialStatement);
            await _context.SaveChangesAsync();
        }
    }
}
