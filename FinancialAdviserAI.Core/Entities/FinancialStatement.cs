using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAdviserAI.Core.Entities
{
    public class FinancialStatement
    {
        [Key]
        public int Id { get; set; }
        public int StockId { get; set; }
        public string StatementType { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Data { get; set; }

        public Stock Stock { get; set; }
    }
}
