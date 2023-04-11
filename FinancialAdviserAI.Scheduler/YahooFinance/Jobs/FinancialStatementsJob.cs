using FinancialAdviserAI.Core.Interfaces.Services;
using Quartz;

namespace FinancialAdviserAI.Scheduler.YahooFinance.Jobs
{
    public class FinancialStatementsJobFactory
    {
        public static IJobDetail BuildJob()
        {
            return JobBuilder.Create<FinancialStatementsJob>()
                .WithIdentity("YahooFinance-Financial-Statements-Job", "YahooFinanceGroup")
                .Build();
        }
    }
    public class FinancialStatementsJob : IJob
    {
        private readonly IFinanceService _financeService;

        public FinancialStatementsJob(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeService.ScrapeFinancialStatementsAsync(cancellationTokenSource.Token);
        }
    }
}
