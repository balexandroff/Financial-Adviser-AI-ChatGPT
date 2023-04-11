using FinancialAdviserAI.Core;
using FinancialAdviserAI.Core.Entities;
using FinancialAdviserAI.Core.Interfaces.Repositories;

namespace FinancialAdviserAI.Data.Repositories
{
    public class FinancialNewsRepository : IFinancialNewsRepository
    {
        private readonly FinanceDataContext _context;

        public FinancialNewsRepository(FinanceDataContext context)
        {
            _context = context;
        }

        public async Task TryAddNewsAsync(FinancialNews financialNews)
        {
            if (!_context.FinancialNews.Any(n => n.Title == financialNews.Title && n.PublicationDate == financialNews.PublicationDate))
            {
                await _context.FinancialNews.AddAsync(financialNews);
                await _context.SaveChangesAsync();
            }
        }
    }
}
