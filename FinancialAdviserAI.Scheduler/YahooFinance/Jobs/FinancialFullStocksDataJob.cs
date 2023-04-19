using FinancialAdviserAI.Core.Interfaces.Services;
using Quartz;

namespace FinancialAdviserAI.Scheduler.YahooFinance.Jobs
{
    public class FinancialFullStocksDataJobFactory
    {
        public static IJobDetail BuildJob()
        {
            return JobBuilder.Create<FinancialFullStocksDataJob>()
                .WithIdentity("YahooFinance-Full-Stock-Data-Job", "YahooFinanceGroup")
                .Build();
        }
    }
    public class FinancialFullStocksDataJob : IJob
    {
        private readonly IFinanceService _financeService;

        public FinancialFullStocksDataJob(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeService.ScrapeFullStocksDataAsync(cancellationTokenSource.Token);
        }
    }
}
