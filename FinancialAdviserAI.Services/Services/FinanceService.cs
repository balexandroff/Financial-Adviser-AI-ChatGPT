using FinancialAdviserAI.Core.Configuration;
using FinancialAdviserAI.Core.Interfaces.Repositories;
using FinancialAdviserAI.Core.Interfaces.Services;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinancialAdviserAI.Services.Services
{
    public class FinanceService : FinanceBaseService, IFinanceService
    {
        private readonly IFinanceStatementsService _financialStatementsService;
        private readonly IFinanceNewsService _financialNewsService;

        public FinanceService(IOptions<DataSourceSettings> dataSourceSettings,
                              ILogger<FinanceService> logger,
                              IFinanceStatementsService financialStatementsService,
                              IFinanceNewsService financialNewsService)
            : base(dataSourceSettings, logger)
        {
            _financialStatementsService = financialStatementsService;
            _financialNewsService = financialNewsService;
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

                                var url = company.SelectSingleNode("td/a").Attributes.Where(attr => attr.Name == "href").Select(attr => attr.Value).FirstOrDefault();
                                
                                await _financialStatementsService.ScrapeFinancialStatementsAsync(cancellationToken, new List<string> { ticker });

                                await _financialNewsService.ScrapeStockMarketCapFinancialNewsAsync(ticker, url, cancellationToken);
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
