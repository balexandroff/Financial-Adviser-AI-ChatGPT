using FinancialAdviserAI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAdviserAI.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _financeService;

        public FinanceController(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        [HttpGet("scrape-news")]
        public async Task<IActionResult> ScrapeFinancialNewsDataAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeService.ScrapeFinancialNewsAsync(cancellationTokenSource.Token);

            return Ok();
        }

        [HttpGet("scrape-statements")]
        public async Task<IActionResult> ScrapeFinancialStatementsDataAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeService.ScrapeFinancialStatementsAsync(cancellationTokenSource.Token);

            return Ok();
        }

        [HttpGet("scrape-stocks")]
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
