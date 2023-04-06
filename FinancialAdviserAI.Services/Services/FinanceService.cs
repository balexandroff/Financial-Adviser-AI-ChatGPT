using FinancialAdviserAI.Core.Entities;
using FinancialAdviserAI.Core.Interfaces.Repositories;
using FinancialAdviserAI.Core.Interfaces.Services;
using HtmlAgilityPack;
using YahooFinanceApi;

namespace FinancialAdviserAI.Services.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IFinancialNewsRepository _financialNewsRepository;
        private readonly IFinancialStatementRepository _financialStatementRepository;
        private readonly IFinancialRatioRepository _financialRatioRepository;

        public FinanceService(IStockRepository stockRepository,
                              IFinancialNewsRepository financialNewsRepository,
                              IFinancialStatementRepository financialStatementRepository,
                              IFinancialRatioRepository financialRatioRepository)
        {
            _stockRepository = stockRepository;
            _financialNewsRepository = financialNewsRepository;
            _financialStatementRepository = financialStatementRepository;
            _financialRatioRepository = financialRatioRepository;
        }

        public async Task ScrapeFinancialDataAsync(CancellationToken cancellationToken)
        {
            var rssArticles = await Yahoo.GetRSSNewsFeedAsync(cancellationToken);
            //var data = await Yahoo.GetHistoricalAsync("AAPL", DateTime.Now.AddYears(-1), DateTime.Now, Period.Daily, cancellationToken);

            foreach (var item in rssArticles)
            {
                var web = new HtmlWeb();
                var doc = web.Load(item.Link);

                //var articleTitle = doc.DocumentNode.Descendants("h1")
                //                                    .FirstOrDefault(n => n.GetAttributeValue("class", "")
                //                                    .Equals("caas-title-wrapper")).InnerText.Trim();

                //var articleAuthor = doc.DocumentNode.Descendants("span")
                //                                     .FirstOrDefault(n => n.GetAttributeValue("class", "")
                //                                     .Equals("caas-attr-meta")).InnerText.Trim();

                //var articleDate = doc.DocumentNode.Descendants("time")
                //                                   .FirstOrDefault(n => n.GetAttributeValue("class", "")
                //                                   .Equals("caas-attr-meta")).GetAttributeValue("datetime", "");

                item.Description = string.Join(Environment.NewLine, doc.DocumentNode.Descendants("p")
                                                                                  .Where(n => !string.IsNullOrWhiteSpace(n.InnerText))
                                                                                  .Select(n => n.InnerText.Trim()));

                Task.Delay(3000).Wait();
            }

            if (rssArticles.Any())
            {
                foreach(var news in rssArticles)
                    await _financialNewsRepository.AddNewsAsync(new FinancialNews
                    {
                        Title = news.Title,
                        Description = news.Description,
                        Link = news.Link,
                        PublicationDate = news.PublicationDate
                    });
            }

            //var stock = new Stock
            //{
            //    Ticker = ticker,
            //    CreatedAt = DateTime.UtcNow
            //};

            //await _stockRepository.AddStockAsync(stock);

            //var financialStatement = new FinancialStatement
            //{
            //    StockId = stock.Id,
            //    StatementType = "income",
            //    Year = 2022,
            //    Quarter = 1,
            //    CreatedAt = DateTime.UtcNow,
            //    Data = "..."
            //};

            //await _financialStatementRepository.AddFinancialStatementAsync(financialStatement);

            //var financialRatio = new FinancialRatio
            //{
            //    StockId = stock.Id,
            //    RatioType = "p/e",
            //    Year = 2022,
            //    Quarter = 1,
            //    CreatedAt = DateTime.UtcNow,
            //    Data = "..."
            //};

            //await _financialRatioRepository.AddFinancialRatioAsync(financialRatio);
        }
    }
}
