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
    public class FinanceNewsService : FinanceBaseService, IFinanceNewsService
    {
        private readonly IFinancialNewsRepository _financialNewsRepository;

        public FinanceNewsService(IOptions<DataSourceSettings> dataSourceSettings,
                              ILogger<FinanceNewsService> logger,
                              IFinancialNewsRepository financialNewsRepository)
            : base(dataSourceSettings, logger)
        {
            _financialNewsRepository = financialNewsRepository;
        }

        public async Task ScrapeYahooFinancialNewsAsync(CancellationToken cancellationToken)
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

                    await Task.Delay(DelayMilliseconds);
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
        }

        public async Task ScrapeStockMarketCapFinancialNewsAsync(string ticker, string url, CancellationToken cancellationToken)
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
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
                            web = new HtmlWeb();
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
        }
    }
}
