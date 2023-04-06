using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAdviserAI.Core.Entities
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Industry { get; set; }
        public string Sector { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<FinancialStatement> FinancialStatements { get; set; }
        public ICollection<FinancialRatio> FinancialRatios { get; set; }
    }
}
