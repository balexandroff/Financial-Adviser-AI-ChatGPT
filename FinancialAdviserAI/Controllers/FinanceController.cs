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

        [HttpGet]
        public async Task<IActionResult> ScrapeFinancialNewsDataAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await _financeService.ScrapeFinancialDataAsync(cancellationTokenSource.Token);

            return Ok();
        }
    }
}
