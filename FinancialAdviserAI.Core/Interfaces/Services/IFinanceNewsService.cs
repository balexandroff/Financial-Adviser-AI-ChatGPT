namespace FinancialAdviserAI.Core.Interfaces.Services
{
    public interface IFinanceNewsService
    {
        Task ScrapeYahooFinancialNewsAsync(CancellationToken cancellationToken);
        Task ScrapeStockMarketCapFinancialNewsAsync(string ticker, string url, CancellationToken cancellationToken);
    }
}
