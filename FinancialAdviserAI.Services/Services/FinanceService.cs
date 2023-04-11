using FinancialAdviserAI.Core.Configuration;
using FinancialAdviserAI.Core.Entities;
using FinancialAdviserAI.Core.Interfaces.Repositories;
using FinancialAdviserAI.Core.Interfaces.Services;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using YahooFinanceApi;

namespace FinancialAdviserAI.Services.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly IOptions<DataSourceSettings> _dataSourceSettings;
        private readonly IStockRepository _stockRepository;
        private readonly IFinancialNewsRepository _financialNewsRepository;
        private readonly IFinancialStatementRepository _financialStatementRepository;
        private readonly IFinancialRatioRepository _financialRatioRepository;

        public FinanceService(IOptions<DataSourceSettings> dataSourceSettings,
                              IStockRepository stockRepository,
                              IFinancialNewsRepository financialNewsRepository,
                              IFinancialStatementRepository financialStatementRepository,
                              IFinancialRatioRepository financialRatioRepository)
        {
            _dataSourceSettings = dataSourceSettings;
            _stockRepository = stockRepository;
            _financialNewsRepository = financialNewsRepository;
            _financialStatementRepository = financialStatementRepository;
            _financialRatioRepository = financialRatioRepository;
        }

        public Stock GetCompanyProfileInfo(string symbol)
        {
            Stock result = new Stock { Ticker = symbol };

            string url = _dataSourceSettings.Value.YahooFinance.ProfileUrl.Replace("{ticker}", symbol);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            HtmlNode companyProfileNode = doc.DocumentNode.SelectSingleNode("//div[@class='asset-profile-container']");

            if (companyProfileNode != null)
            {
                result.Name = companyProfileNode.SelectSingleNode("//h3[@class='Fz(m) Mb(10px)']").InnerText;
                if (companyProfileNode.SelectSingleNode("//p[@class='D(ib) W(47.727%) Pend(40px)']").ChildNodes.Count >= 5)
                    result.Country = companyProfileNode.SelectSingleNode("//p[@class='D(ib) W(47.727%) Pend(40px)']").ChildNodes[4].InnerText;
                if (companyProfileNode.SelectSingleNode("//p[@class='D(ib) Va(t)']").ChildNodes.Count >= 4)
                    result.Industry = companyProfileNode.SelectSingleNode("//p[@class='D(ib) Va(t)']").ChildNodes.Where(e => e.Name == "span").ToList()[3].InnerText;
                if (companyProfileNode.SelectSingleNode("//p[@class='D(ib) Va(t)']").ChildNodes.Count >= 2)
                    result.Sector = companyProfileNode.SelectSingleNode("//p[@class='D(ib) Va(t)']").ChildNodes.Where(e => e.Name == "span").ToList()[1].InnerText;
                result.CreatedAt = DateTime.Now;
            }
            
            return result;
        }

        public async Task ScrapeFinancialNewsAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(async () =>
            {
                var rssArticles = await Yahoo.GetRSSNewsFeedAsync(cancellationToken);
                foreach (var item in rssArticles)
                {
                    var web = new HtmlWeb();
                    var doc = web.Load(item.Link);

                    item.Description = string.Join(Environment.NewLine, doc.DocumentNode.Descendants("p")
                                                                                        .Where(n => !string.IsNullOrWhiteSpace(n.InnerText))
                                                                                        .Select(n => n.InnerText.Trim()));

                    Task.Delay(3000).Wait();
                }

                if (rssArticles.Any())
                {
                    foreach (var news in rssArticles)
                        await _financialNewsRepository.TryAddNewsAsync(new FinancialNews
                        {
                            Title = news.Title,
                            Description = news.Description,
                            Link = news.Link,
                            PublicationDate = news.PublicationDate
                        });
                }
            });
        }

        public async Task ScrapeFinancialStatementsAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(async () =>
            {
                foreach (var ticker in _dataSourceSettings.Value.Stocks.Split(','))
                {
                    HttpClient httpClient = new HttpClient();
                    var stock = await _stockRepository.TryAddStockAsync(GetCompanyProfileInfo(ticker));

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
                    }

                    await Task.Delay(3000);

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
                    }

                    await Task.Delay(3000);

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
                    }

                    await Task.Delay(3000);
                }
            });
        }
    }
}
