using FinancialAdviserAI.Core.Interfaces.Services;
using Quartz;

namespace FinancialAdviserAI.Scheduler.YahooFinance.Jobs
{
    public class RSSNewsFeedJobFactory
    {
        public static IJobDetail BuildJob()
        {
            return JobBuilder.Create<RSSNewsFeedJob>()
                .WithIdentity("YahooFinance-RSS-News-Feed-Job", "YahooFinanceGroup")
                .Build();
        }
    }
    public class RSSNewsFeedJob : IJob
    {
        private readonly IFinanceService _financeService;

        public RSSNewsFeedJob(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeService.ScrapeFinancialDataAsync(cancellationTokenSource.Token);
        }
    }
}
