using Quartz;

namespace FinancialAdviserAI.Scheduler.YahooFinance.Triggers
{
    public class RSSNewsFeedTriggerFactory
    {
        public static ITrigger BuildTrigger()
        {
            return TriggerBuilder.Create()
                .WithIdentity("YahooFinance-RSS-News-Feed-Trigger", "YahooFinanceGroup")
                .StartNow()
                .WithCronSchedule("0 0 0/6 ? * * *")
                .Build();
        }
    }
}
