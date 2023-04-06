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
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            IJobDetail job = RSSNewsFeedJobFactory.BuildJob();
            ITrigger trigger = RSSNewsFeedTriggerFactory.BuildTrigger();

            await scheduler.ScheduleJob(job, trigger);

            await scheduler.Start();

            // Trigger the job to run immediately
            await scheduler.TriggerJob(job.Key);

            return app;
        }
    }
}
