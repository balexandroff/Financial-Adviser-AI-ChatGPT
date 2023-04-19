using FinancialAdviserAI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAdviserAI.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _financeService;
        private readonly IFinanceNewsService _financeNewsService;
        private readonly IFinanceStatementsService _financeStatementsService;

        public FinanceController(IFinanceService financeService, IFinanceNewsService financeNewsService, 
            IFinanceStatementsService financeStatementsService)
        {
            _financeService = financeService;
            _financeNewsService = financeNewsService;
            _financeStatementsService = financeStatementsService;
        }

        [HttpGet("scrape-news-yahoo")]
        public async Task<IActionResult> ScrapeYahooFinancialNewsDataAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeNewsService.ScrapeYahooFinancialNewsAsync(cancellationTokenSource.Token);

            return Ok();
        }

        [HttpGet("scrape-statements/{ticker}")]
        public async Task<IActionResult> ScrapeFinancialStatementsDataAsync(string ticker)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeStatementsService.ScrapeFinancialStatementsAsync(cancellationTokenSource.Token, new List<string> { ticker });

            return Ok();
        }

        [HttpGet("scrape-stocks-all")]
        public async Task<IActionResult> ScrapeStocksFromMarketAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeService.ScrapeFullStocksDataAsync(cancellationTokenSource.Token);

            return Ok();
        }

        [HttpGet("scrape-stocks/{ticker}")]
        public async Task<IActionResult> ScrapeStocksFromMarketAsync(string ticker)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeService.ScrapeFullStocksDataAsync(cancellationTokenSource.Token, ticker);

            return Ok();
        }
    }
}
