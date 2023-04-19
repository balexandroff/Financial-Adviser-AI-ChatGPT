using Quartz;

namespace FinancialAdviserAI.Scheduler.YahooFinance.Triggers
{
    public class FinancialFullStocksDataTriggerFactory
    {
        public static ITrigger BuildTrigger()
        {
            return TriggerBuilder.Create()
                .WithIdentity("YahooFinance-Financial-Full-Stocks-Data-Trigger", "YahooFinanceGroup")
                .StartNow()
                .WithCronSchedule("0 0 * * * ?")
                .Build();
        }
    }
}
