using FinancialAdviserAI.Core;
using FinancialAdviserAI.Core.Entities;
using FinancialAdviserAI.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinancialAdviserAI.Data.Repositories
{
    public class FinancialStatementRepository : IFinancialStatementRepository
    {
        private readonly FinanceDataContext _context;

        public FinancialStatementRepository(FinanceDataContext context)

        {
            _context = context;
        }

        public async Task<FinancialStatement> TryAddFinancialStatementAsync(FinancialStatement financialStatement)
        {
            var entity = await _context.FinancialStatements.FirstOrDefaultAsync(s => s.StockId == financialStatement.StockId &&
                                                                                     s.StatementType == financialStatement.StatementType &&
                                                                                     s.Quarter == financialStatement.Quarter &&
                                                                                     s.Year == financialStatement.Year);

            if (entity == null)
            {
                await _context.FinancialStatements.AddAsync(financialStatement);
                await _context.SaveChangesAsync();

                return await _context.FinancialStatements.FirstOrDefaultAsync(s => s.StockId == financialStatement.StockId &&
                                                                                    s.StatementType == financialStatement.StatementType &&
                                                                                    s.Quarter == financialStatement.Quarter &&
                                                                                    s.Year == financialStatement.Year);
            }
            else
            {
                return entity;
            }
        }
    }
}
