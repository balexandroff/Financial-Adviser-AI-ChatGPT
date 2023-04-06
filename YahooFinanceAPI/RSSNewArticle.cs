using System;

namespace YahooFinanceApi
{
    public sealed class RSSNewArticle
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
