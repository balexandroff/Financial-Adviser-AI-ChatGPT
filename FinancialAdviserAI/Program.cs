using FinancialAdviserAI.Core;
using FinancialAdviserAI.Core.Configuration;
using FinancialAdviserAI.Core.Interfaces.Repositories;
using FinancialAdviserAI.Core.Interfaces.Services;
using FinancialAdviserAI.Data.Repositories;
using FinancialAdviserAI.Extensions;
using FinancialAdviserAI.Scheduler.YahooFinance.Jobs;
using FinancialAdviserAI.Services.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOptions<DataSourceSettings>().Bind(builder.Configuration.GetSection("DataSources"));

// Register your DbContext here
builder.Services.AddDbContext<FinanceDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinanceConnectionString")));

// Register your repositories and services here
builder.Services.AddScoped<IFinancialNewsRepository, FinancialNewsRepository>();
builder.Services.AddScoped<IFinancialRatioRepository, FinancialRatioRepository>();
builder.Services.AddScoped<IFinancialStatementRepository, FinancialStatementRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();

builder.Services.AddScoped<IFinanceService, FinanceService>();

builder.Services.AddScoped<IJob, RSSNewsFeedJob>();
builder.Services.AddScoped<IJob, FinancialStatementsJob>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
});
builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Task.Factory.StartNew(async () => {
    await Task.Delay(5000);
    await app.AddBackgroudJobs();
});

app.Run();
