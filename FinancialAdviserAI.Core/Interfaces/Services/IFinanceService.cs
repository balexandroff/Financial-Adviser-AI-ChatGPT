namespace FinancialAdviserAI.Core.Interfaces.Services
{
    public interface IFinanceService
    {
        Task ScrapeFullStocksDataAsync(CancellationToken cancellationToken, string ticker = null);
    }
}
