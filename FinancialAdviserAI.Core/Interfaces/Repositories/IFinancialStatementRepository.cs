using FinancialAdviserAI.Core.Entities;

namespace FinancialAdviserAI.Core.Interfaces.Repositories
{
    public interface IFinancialStatementRepository
    {
        Task AddFinancialStatementAsync(FinancialStatement financialStatement);
    }
}
