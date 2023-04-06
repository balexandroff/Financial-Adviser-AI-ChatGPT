using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace YahooFinanceApi
{
    public sealed partial class Yahoo
    {
        public static async Task<IReadOnlyList<RSSNewArticle>> GetRSSNewsFeedAsync(CancellationToken token = default)
        {
            var sindicationRead = await _GetResponseSindicationReadAsync(token).ConfigureAwait(false);

            return await Task.FromResult(sindicationRead.Items.Select(i => new RSSNewArticle
            {
                Title = i.Title.Text,
                Link = i.Links.Select(l => l.Uri.ToString()).FirstOrDefault(),
                PublicationDate = i.PublishDate.DateTime
            }).ToList());
        }

        private static async Task<SyndicationFeed> _GetResponseSindicationReadAsync(CancellationToken _token)
        {
            var url = "https://finance.yahoo.com/news/rss";

            using var reader = XmlReader.Create(url);

            Debug.WriteLine(url);

            return await Task.FromResult(SyndicationFeed.Load(reader));
        }
    }
}
