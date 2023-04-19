using FinancialAdviserAI.Core.Configuration;
using FinancialAdviserAI.Core.Entities;
using FinancialAdviserAI.Core.Interfaces.Repositories;
using FinancialAdviserAI.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinancialAdviserAI.Services.Services
{
    public class FinanceStatementsService : FinanceBaseService, IFinanceStatementsService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IFinancialStatementRepository _financialStatementRepository;

        public FinanceStatementsService(IOptions<DataSourceSettings> dataSourceSettings,
                              ILogger<FinanceStatementsService> logger,
                              IStockRepository stockRepository,
                              IFinancialStatementRepository financialStatementRepository)
            : base(dataSourceSettings, logger)
        {
            _stockRepository = stockRepository;
            _financialStatementRepository = financialStatementRepository;
        }

        public async Task ScrapeFinancialStatementsAsync(CancellationToken cancellationToken, IEnumerable<string> stocks = null)
        {
            foreach (var ticker in stocks ?? _dataSourceSettings.Value.Stocks.Split(','))
            {
                try
                {
                    HttpClient httpClient = new HttpClient();
                    var stock = await _stockRepository.TryAddStockAsync(await base.GetCompanyProfileInfo(ticker));

                    _logger.LogInformation($"Created/Updated stock {ticker}");

                    //Fetch Income Statements
                    string url = _dataSourceSettings.Value.YahooFinance.IncomeStatementsUrl.Replace("{ticker}", ticker);

                    var jsonData = await httpClient.GetStringAsync(url);

                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        await _financialStatementRepository.TryAddFinancialStatementAsync(new FinancialStatement
                        {
                            StockId = stock.Id,
                            StatementType = "Income Statement",
                            Quarter = (int)Math.Ceiling((double)DateTime.Now.AddMonths(-3).Month / (double)3), // Prev quarter
                            Year = DateTime.Now.AddMonths(-3).Year,
                            CreatedAt = DateTime.Now,
                            Data = jsonData
                        });

                        _logger.LogInformation($"Added Income Statement for stock {ticker}");
                    }

                    await Task.Delay(DelayMilliseconds);

                    //Fetch Balance Sheet
                    url = _dataSourceSettings.Value.YahooFinance.BalanceSheetUrl.Replace("{ticker}", ticker);
                    jsonData = await httpClient.GetStringAsync(url);

                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        await _financialStatementRepository.TryAddFinancialStatementAsync(new FinancialStatement
                        {
                            StockId = stock.Id,
                            StatementType = "Balance Sheet",
                            Quarter = (int)Math.Ceiling((double)DateTime.Now.AddMonths(-3).Month / (double)3), // Prev quarter
                            Year = DateTime.Now.AddMonths(-3).Year,
                            CreatedAt = DateTime.Now,
                            Data = jsonData
                        });

                        _logger.LogInformation($"Added Balance Sheet for stock {ticker}");
                    }

                    await Task.Delay(DelayMilliseconds);

                    //Fetch Cash Flow
                    url = _dataSourceSettings.Value.YahooFinance.CashFlowUrl.Replace("{ticker}", ticker);
                    jsonData = await httpClient.GetStringAsync(url);

                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        await _financialStatementRepository.TryAddFinancialStatementAsync(new FinancialStatement
                        {
                            StockId = stock.Id,
                            StatementType = "Cash Flow",
                            Quarter = (int)Math.Ceiling((double)DateTime.Now.AddMonths(-3).Month / (double)3), // Prev quarter
                            Year = DateTime.Now.AddMonths(-3).Year,
                            CreatedAt = DateTime.Now,
                            Data = jsonData
                        });

                        _logger.LogInformation($"Added Cash Flow for stock {ticker}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                finally
                {
                    await Task.Delay(DelayMilliseconds);
                }
            }
        }
    }
}
