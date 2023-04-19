using FinancialAdviserAI.Scheduler.YahooFinance.Jobs;
using FinancialAdviserAI.Scheduler.YahooFinance.Triggers;
using Quartz;
using Quartz.Impl;

namespace FinancialAdviserAI.Extensions
{
    public static class Extensions
    {
        public static async Task<WebApplication> AddBackgroudJobs(this WebApplication app)
        {
            var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
            IScheduler scheduler = await schedulerFactory.GetScheduler();

            // Full Stocks Data
            IJobDetail fullStockDataJob = FinancialFullStocksDataJobFactory.BuildJob();
            ITrigger fullStockDataTrigger = FinancialFullStocksDataTriggerFactory.BuildTrigger();
            await scheduler.ScheduleJob(fullStockDataJob, fullStockDataTrigger);

            // RSS News
            IJobDetail rssJob = RSSNewsFeedJobFactory.BuildJob();
            ITrigger rssTrigger = RSSNewsFeedTriggerFactory.BuildTrigger();
            await scheduler.ScheduleJob(rssJob, rssTrigger);

            // Financial Statements
            IJobDetail finStatejob = FinancialStatementsJobFactory.BuildJob();
            ITrigger finStatetrigger = FinancialStatementsTriggerFactory.BuildTrigger();
            await scheduler.ScheduleJob(finStatejob, finStatetrigger);

            await scheduler.Start();

            // Trigger the job to run immediately
            await scheduler.TriggerJob(fullStockDataJob.Key);
            //await Task.Delay(5000);
            //await scheduler.TriggerJob(rssJob.Key);
            //await Task.Delay(5000);
            //await scheduler.TriggerJob(finStatejob.Key);

            return app;
        }
    }
}
