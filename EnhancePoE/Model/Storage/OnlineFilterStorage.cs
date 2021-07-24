using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EnhancePoE.Model.Storage
{
    public class OnlineFilterStorage : IFilterStorage
    {
        private readonly string _filterName;
        private readonly string _accName;
        private readonly string _sessionId;

        private const string FilterUrlXPathTemplate =
            "//div[@class='item-filter' and h3/a/text()='{0}']/div[@class='item-filter-buttons']/a[@class='button-text' and text()='Edit']";
        public static readonly Uri FiltersUrl = new Uri("https://www.pathofexile.com/account/view-profile/{0}/item-filters");
        private string _filterUrl;
        private Dictionary<string, string> _filterData;

        internal OnlineFilterStorage(string filterName, string accName, string sessionId)
        {
            _filterName = filterName;
            _accName = accName;
            _sessionId = sessionId;
        }

        public async Task<string> ReadLootFilterAsync()
        {
            var filterUrl = await GetFilterUrlAsync();

            if (string.IsNullOrWhiteSpace(filterUrl))
            {
                return null;
            }

            using (var handler = GetHandler(filterUrl))
            using (HttpClient client = new HttpClient(handler))
            {
                var response = await client.GetAsync(filterUrl);

                var stream = await response.Content.ReadAsStreamAsync();
                HtmlDocument document = new HtmlDocument();
                document.Load(stream, Encoding.UTF8);
                _filterData = ParseToDictionary(document);

                return _filterData["filter"];
            }
        }

        public async Task WriteLootFilterAsync(string filter)
        {
            if (_filterData == null)
            {
                await ReadLootFilterAsync();
            }

            var filterUrl = await GetFilterUrlAsync();

            _filterData["filter"] = filter;
            _filterData["version"] = "g_" + DateTime.UtcNow.ToString("yyyyMMdd_hhmmss", CultureInfo.InvariantCulture);

            using (var handler = GetHandler(filterUrl))
            using (HttpClient client = new HttpClient(handler))
            {
                // Cannot use FormUrlEncodedContent because it has limitations on variable sizes
                var encodedItems = _filterData.Select(i => WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value));
                var content = new StringContent(string.Join("&", encodedItems), null, "application/x-www-form-urlencoded");

                await client.PostAsync(filterUrl, content);

                _filterData = null;
            }
        }

        private async Task<string> GetFilterUrlAsync()
        {
            if (_filterUrl == null)
            {
                string url = string.Format(FiltersUrl.ToString(), _accName);
                using (var handler = GetHandler(url))
                using (HttpClient client = new HttpClient(handler))
                {
                    var response = await client.GetAsync(url).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        HtmlDocument document = new HtmlDocument();
                        document.Load(stream, Encoding.UTF8);

                        var xpath = string.Format(FilterUrlXPathTemplate, _filterName);
                        _filterUrl = document.DocumentNode.SelectSingleNode(xpath)?.GetAttributeValue("href", null);

                        if (_filterUrl != null)
                        {
                            _filterUrl = "https://www.pathofexile.com" + _filterUrl;
                        }
                    }
                }
            }

            return _filterUrl;
        }

        private HttpClientHandler GetHandler(string url)
        {
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri(url), new Cookie("POESESSID", _sessionId));
            return new HttpClientHandler { CookieContainer = cookieContainer };
        }

        private Dictionary<string, string> ParseToDictionary(HtmlDocument document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>
            {
                ["filter_name"] = document.DocumentNode.SelectSingleNode("//input[@name='filter_name']").GetAttributeValue("value", ""),
                ["realm"] = document.DocumentNode.SelectSingleNode("//select[@name='realm']/option[@selected]").GetAttributeValue("value", null),
                ["public"] = document.DocumentNode.SelectSingleNode("//input[@id='public']").Attributes.Any(a => a.Name == "checked") ? "1" : "0",
                ["description"] = WebUtility.HtmlDecode(document.DocumentNode.SelectSingleNode("//textarea[@name='description']").InnerText.Trim()),
                ["version"] = "",
                ["should_validate"] = document.DocumentNode.SelectSingleNode("//input[@id='should_validate']").Attributes.Any(a => a.Name == "checked") ? "1" : "0",
                ["filter"] = WebUtility.HtmlDecode(document.DocumentNode.SelectSingleNode("//textarea[@name='filter']").InnerText.Trim()),
                ["copied_from"] = document.DocumentNode.SelectSingleNode("//input[@name='copied_from']").GetAttributeValue("value", null),
                ["hash"] = document.DocumentNode.SelectSingleNode("//input[@name='hash']").GetAttributeValue("value", null),
                ["submit"] = "Submit"
            };


            return result;
        }
    }
}
