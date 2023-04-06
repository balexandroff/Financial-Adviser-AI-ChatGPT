namespace FinancialAdviserAI.Core.Interfaces.Services
{
    public interface IFinanceService
    {
        Task ScrapeFinancialDataAsync(CancellationToken cancellationToken);
    }
}
