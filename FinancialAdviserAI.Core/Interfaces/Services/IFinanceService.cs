namespace FinancialAdviserAI.Core.Interfaces.Services
{
    public interface IFinanceService
    {
        Task ScrapeFinancialNewsAsync(CancellationToken cancellationToken);

        Task ScrapeFinancialStatementsAsync(CancellationToken cancellationToken, IEnumerable<string> stocks = null);

        Task ScrapeFullStocksDataAsync(CancellationToken cancellationToken, string ticker = null);
    }
}
