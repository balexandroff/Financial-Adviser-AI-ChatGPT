using FinancialAdviserAI.Core.Configuration;
using FinancialAdviserAI.Core.Entities;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinancialAdviserAI.Services.Services
{
    public abstract class FinanceBaseService
    {
        protected const int DelayMilliseconds = 2000;

        protected readonly ILogger<FinanceBaseService> _logger;
        protected readonly IOptions<DataSourceSettings> _dataSourceSettings;

        public FinanceBaseService(IOptions<DataSourceSettings> dataSourceSettings, ILogger<FinanceBaseService> logger)
        {
            _logger = logger;
            _dataSourceSettings = dataSourceSettings;
        }

        public async Task<Stock> GetCompanyProfileInfo(string symbol)
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

                return await Task.FromResult(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult(result);
            }
        }
    }
}
