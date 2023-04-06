using FinancialAdviserAI.Core.Entities;

namespace FinancialAdviserAI.Core.Interfaces.Repositories
{
    public interface IFinancialNewsRepository
    {
        Task AddNewsAsync(FinancialNews financialNews);
    }
}
