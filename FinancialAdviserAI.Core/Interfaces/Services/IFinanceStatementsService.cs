namespace FinancialAdviserAI.Core.Interfaces.Services
{
    public interface IFinanceStatementsService
    {
        Task ScrapeFinancialStatementsAsync(CancellationToken cancellationToken, IEnumerable<string> stocks = null);
    }
}
