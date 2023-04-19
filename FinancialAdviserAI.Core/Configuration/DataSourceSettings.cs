namespace FinancialAdviserAI.Core.Configuration
{
    public class DataSourceSettings
    {
        public string StockMarketUrl { get; set; }
        public string Stocks { get; set; }
        public YahooFinanceSettings YahooFinance { get; set; }

        public class YahooFinanceSettings
        {
            public string ProfileUrl { get; set; }
            public string NewsUrl { get; set; }
            public string IncomeStatementsUrl { get; set; }
            public string BalanceSheetUrl { get; set; }
            public string CashFlowUrl { get; set; }
        }
    }
}
