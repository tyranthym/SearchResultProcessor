using SearchResultProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchResultProcessor.Services
{
    public interface IScrapingService
    {
        /// <summary>
        /// save page source to a fileg
        /// </summary>
        /// <param name="pageSource"></param>
        /// <param name="keywords"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        Task SaveToFileAsync(string pageSource, string keywords, DateTime date);

        /// <summary>
        /// Getting a string of number for where the resulting url is found in google search 
        /// </summary>
        /// <param name="googleSearchResults"></param>
        /// <returns></returns>
        string GetPositionsFromMatchingResults(List<GoogleSearchResult> googleSearchResults);

        /// <summary>
        /// build google search url
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="keywords"></param>
        /// <param name="numOfResult">num of results in one page</param>
        /// <returns></returns>
        string BuildGoogleSearchUrl(string baseUrl, string keywords, int numOfResult);
    }
}
