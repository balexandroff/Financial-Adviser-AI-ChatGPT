using FinancialAdviserAI.Core.Configuration;
using FinancialAdviserAI.Core.Entities;
using FinancialAdviserAI.Core.Interfaces.Repositories;
using FinancialAdviserAI.Core.Interfaces.Services;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YahooFinanceApi;

namespace FinancialAdviserAI.Services.Services
{
    public class FinanceService : IFinanceService
    {
        private const int DelayMilliseconds = 2000;

        private readonly ILogger<FinanceService> _logger;
        private readonly IOptions<DataSourceSettings> _dataSourceSettings;
        private readonly IStockRepository _stockRepository;
        private readonly IFinancialNewsRepository _financialNewsRepository;
        private readonly IFinancialStatementRepository _financialStatementRepository;
        private readonly IFinancialRatioRepository _financialRatioRepository;

        public FinanceService(IOptions<DataSourceSettings> dataSourceSettings,
                              ILogger<FinanceService> logger,
                              IStockRepository stockRepository,
                              IFinancialNewsRepository financialNewsRepository,
                              IFinancialStatementRepository financialStatementRepository,
                              IFinancialRatioRepository financialRatioRepository)
        {
            _logger = logger;
            _dataSourceSettings = dataSourceSettings;
            _stockRepository = stockRepository;
            _financialNewsRepository = financialNewsRepository;
            _financialStatementRepository = financialStatementRepository;
            _financialRatioRepository = financialRatioRepository;
        }

        public Stock GetCompanyProfileInfo(string symbol)
        {
            Stock result = new Stock { Ticker = symbol };

            try
            {
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

                result.Name = result.Name ?? result.Ticker;
                result.Country = result.Country ?? "N/A";
                result.Industry = result.Industry ?? "N/A";
                result.Sector = result.Sector ?? "N/A";

                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result;
            }
        }

        public async Task ScrapeFinancialNewsAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    var rssArticles = await Yahoo.GetRSSNewsFeedAsync(cancellationToken);
                    foreach (var item in rssArticles)
                    {
                        var web = new HtmlWeb();
                        var doc = web.Load(item.Link);

                        item.Description = string.Join(Environment.NewLine, doc.DocumentNode.Descendants("p")
                                                                                            .Where(n => !string.IsNullOrWhiteSpace(n.InnerText))
                                                                                            .Select(n => n.InnerText.Trim()));

                        Task.Delay(DelayMilliseconds).Wait();
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

                        _logger.LogInformation($"Added Yahoo RSS financial data for generic stocks.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            });
        }

        public async Task ScrapeFinancialStatementsAsync(CancellationToken cancellationToken, IEnumerable<string> stocks = null)
        {
            await Task.Factory.StartNew(async () =>
            {
                foreach (var ticker in stocks ?? _dataSourceSettings.Value.Stocks.Split(','))
                {
                    try
                    {
                        HttpClient httpClient = new HttpClient();
                        var stock = await _stockRepository.TryAddStockAsync(GetCompanyProfileInfo(ticker));

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
            });
        }

        public async Task ScrapeFullStocksDataAsync(CancellationToken cancellationToken, string ticker = null)
        {
            HtmlWeb web = new HtmlWeb();
            for (int i = 1; i < 100; i++)
            {
                string pageUrl = _dataSourceSettings.Value.StockMarketUrl.Replace("{page}", i.ToString());

                HtmlDocument doc = web.Load(pageUrl);

                List<HtmlNode> companyProfiles = new List<HtmlNode>();

                try
                {
                    if (string.IsNullOrEmpty(ticker))
                    {
                        companyProfiles = doc.DocumentNode.SelectNodes("//table[@id='order-listing']/tbody/tr").ToList();
                    }
                    else
                    {
                        companyProfiles = doc.DocumentNode.SelectNodes("//table[@id='order-listing']/tbody/tr").Where(tr => tr.SelectNodes("div[@class='media-body']/p").Any(cn => cn.InnerText.Contains(ticker))).ToList();
                    }

                    if (companyProfiles != null && companyProfiles.Any())
                    {
                        foreach (var company in companyProfiles)
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(ticker))
                                    ticker = company.SelectSingleNode("td//div[@class='media-body']/p").InnerText.Trim();

                                //string url = companyUrl.Attributes.Where(attr => attr.Name == "href").Select(attr => attr.Value).FirstOrDefault();
                                //HtmlDocument companyDov = web.Load(url);
                                await ScrapeFinancialStatementsAsync(cancellationToken, new List<string> { ticker });

                                await Task.Factory.StartNew(async () =>
                                {
                                    try
                                    {
                                        var url = company.SelectSingleNode("td/a").Attributes.Where(attr => attr.Name == "href").Select(attr => attr.Value).FirstOrDefault();
                                        var companyDoc = web.Load(url);

                                        var newsFeed = companyDoc.DocumentNode.SelectNodes("//div[@id='news-slider']/div[@class='post-slide']/div[@class='post-content']");

                                        if (newsFeed != null && newsFeed.Any())
                                        {
                                            List<FinancialNews> newsArticles = new List<FinancialNews>();
                                            foreach (var news in newsFeed)
                                            {
                                                try
                                                {
                                                    url = news.SelectSingleNode("a").Attributes.Where(attr => attr.Name == "href").Select(attr => attr.Value).FirstOrDefault();
                                                    var web = new HtmlWeb();
                                                    var doc = web.Load(url);

                                                    newsArticles.Add(new FinancialNews
                                                    {
                                                        Title = news.SelectSingleNode("h3[@class='post-title']").InnerText.Trim(),
                                                        Link = url,
                                                        Description = string.Join(Environment.NewLine, doc.DocumentNode.Descendants("p")
                                                                                                                    .Where(n => !string.IsNullOrWhiteSpace(n.InnerText))
                                                                                                                    .Select(n => n.InnerText.Trim())),
                                                        PublicationDate = DateTime.Parse(news.SelectSingleNode("span[@class='post-date']").InnerText.Trim())
                                                    });

                                                }
                                                catch (Exception ex)
                                                {
                                                    _logger.LogError(ex, ex.Message);
                                                }
                                                finally
                                                {
                                                    Task.Delay(DelayMilliseconds).Wait();
                                                }
                                            }

                                            if (newsArticles.Any())
                                            {
                                                foreach (var article in newsArticles)
                                                    await _financialNewsRepository.TryAddNewsAsync(article);

                                                _logger.LogInformation($"News for stock {ticker} added/updated successfully.");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, ex.Message);
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, ex.Message);
                            }
                            finally
                            {
                                await Task.Delay(DelayMilliseconds);
                                ticker = null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}
