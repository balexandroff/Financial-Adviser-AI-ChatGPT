using FinancialAdviserAI.Core.Entities;

namespace FinancialAdviserAI.Core.Interfaces.Repositories
{
    public interface IFinancialRatioRepository
    {
        Task TryAddFinancialRatioAsync(FinancialRatio financialRatio);
    }
}
