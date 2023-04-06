using FinancialAdviserAI.Core.Entities;

namespace FinancialAdviserAI.Core.Interfaces.Repositories
{
    public interface IStockRepository
    {
        Task<Stock> GetStockByTickerAsync(string ticker);
        Task AddStockAsync(Stock stock);
    }
}
