using FinancialAdviserAI.Core.Configuration;
using FinancialAdviserAI.Core.Interfaces.Repositories;
using FinancialAdviserAI.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinancialAdviserAI.Services.Services
{
    public class FinanceRatioService : FinanceBaseService, IFinanceRatiosService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IFinancialNewsRepository _financialNewsRepository;
        private readonly IFinancialStatementRepository _financialStatementRepository;
        private readonly IFinancialRatioRepository _financialRatioRepository;

        public FinanceRatioService(IOptions<DataSourceSettings> dataSourceSettings,
                              ILogger<FinanceRatioService> logger,
                              IStockRepository stockRepository,
                              IFinancialNewsRepository financialNewsRepository,
                              IFinancialStatementRepository financialStatementRepository,
                              IFinancialRatioRepository financialRatioRepository)
            : base(dataSourceSettings, logger)
        {
            _stockRepository = stockRepository;
            _financialNewsRepository = financialNewsRepository;
            _financialStatementRepository = financialStatementRepository;
            _financialRatioRepository = financialRatioRepository;
        }
    }
}
