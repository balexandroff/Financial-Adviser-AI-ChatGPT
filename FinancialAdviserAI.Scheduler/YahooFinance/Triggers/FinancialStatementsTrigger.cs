using Quartz;

namespace FinancialAdviserAI.Scheduler.YahooFinance.Triggers
{
    public class FinancialStatementsTriggerFactory
    {
        public static ITrigger BuildTrigger()
        {
            return TriggerBuilder.Create()
                .WithIdentity("YahooFinance-FinancialStatements-Trigger", "YahooFinanceGroup")
                .StartNow()
                .WithCronSchedule("0 0 0/6 ? * * *")
                .Build();
        }
    }
}
