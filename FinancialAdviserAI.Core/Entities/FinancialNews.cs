using System.ComponentModel.DataAnnotations;

namespace FinancialAdviserAI.Core.Entities
{
    public class FinancialNews
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
