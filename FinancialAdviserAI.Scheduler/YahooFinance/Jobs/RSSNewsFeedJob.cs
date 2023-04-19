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
        private readonly IFinanceNewsService _financeNewsService;

        public RSSNewsFeedJob(IFinanceNewsService financeNewsService)
        {
            _financeNewsService = financeNewsService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeNewsService.ScrapeYahooFinancialNewsAsync(cancellationTokenSource.Token);
        }
    }
}
