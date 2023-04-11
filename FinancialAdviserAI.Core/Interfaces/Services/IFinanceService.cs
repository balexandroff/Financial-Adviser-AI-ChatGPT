namespace FinancialAdviserAI.Core.Interfaces.Services
{
    public interface IFinanceService
    {
        Task ScrapeFinancialNewsAsync(CancellationToken cancellationToken);

        Task ScrapeFinancialStatementsAsync(CancellationToken cancellationToken);
    }
}
