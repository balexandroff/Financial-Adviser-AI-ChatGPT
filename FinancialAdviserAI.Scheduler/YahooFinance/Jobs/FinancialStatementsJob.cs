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
        private readonly IFinanceStatementsService _financeStatementsService;

        public FinancialStatementsJob(IFinanceStatementsService financeStatementsService)
        {
            _financeStatementsService = financeStatementsService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeStatementsService.ScrapeFinancialStatementsAsync(cancellationTokenSource.Token);
        }
    }
}
