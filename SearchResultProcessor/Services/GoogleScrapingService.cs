using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using SearchResultProcessor.Models;
using SearchResultProcessor.Models.CustomExtensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SearchResultProcessor.Services
{
    public class GoogleScrapingService : IScrapingService
    {
        async Task IScrapingService.SaveToFileAsync(string pageSource, string keywords, DateTime date)
        {
            FileInfo file = new FileInfo($"ResultHistory/{keywords}_{date.ToStringSortableShortDash()}.html");
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            await File.WriteAllTextAsync(file.FullName, pageSource);
        }

        string IScrapingService.GetPositionsFromMatchingResults(List<GoogleSearchResult> googleSearchResults)
        {
            List<int> indexes = googleSearchResults.OrderBy(gsr => gsr.Index).Select(gsr => gsr.Index).Distinct().ToList();
            if (indexes.Count == 0)
            {
                return "no matching result found!";
            }
            return Calculator.Concatenate(indexes, ", ");
        }

        public string GetBaseUrl(Uri uri, UriFormat uriFormat = UriFormat.UriEscaped)
        {
            return uri.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, uriFormat);
        }

        public Dictionary<string, StringValues> GetQueryComponentForUri(Uri uri)
        {
            return QueryHelpers.ParseQuery(uri.Query);
        }

        string IScrapingService.BuildGoogleSearchUrl(string baseUrl, string keywords, int numOfResult)
        {
            var baseUri = new Uri(baseUrl);

            baseUrl = GetBaseUrl(baseUri);
            var query = GetQueryComponentForUri(baseUri);

            var queryBuilder = new QueryBuilder();
            queryBuilder.Add("q", keywords);
            queryBuilder.Add("num", numOfResult.ToString());

            return baseUrl + queryBuilder.ToQueryString();
        }
    }
}
