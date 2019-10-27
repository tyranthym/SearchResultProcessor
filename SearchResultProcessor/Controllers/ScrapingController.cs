using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SearchResultProcessor.Models;
using SearchResultProcessor.Models.Constants;
using SearchResultProcessor.Models.CustomExtensions;
using SearchResultProcessor.Models.Requests;
using SearchResultProcessor.Models.Responses;
using SearchResultProcessor.Services;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace SearchResultProcessor.Controllers
{
    [SwaggerTag("Scraping data from http://www.google.com.au")]
    public class ScrapingController : ApiBaseController
    {
        private readonly IWebDriver _webDriver;
        private readonly IScrapingService _scrapingService;
        private static readonly ILogger logger = Log.ForContext<ScrapingController>();

        public ScrapingController(IWebDriver webDriver, IScrapingService scrapingService)
        {
            this._webDriver = webDriver;
            this._scrapingService = scrapingService;
        }

        /// <summary>
        /// scraping google search results, return the matching positions
        /// </summary>
        /// <param name="scrapeRequest"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ScrapeResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Get([FromQuery] ScrapeRequest scrapeRequest)
        {
            //input validation has already handled here 

            string keywords = scrapeRequest.Keywords;
            string matchingKeywords = scrapeRequest.MatchingKeywords;
            string baseUrl = "https://www.google.com.au/search";
            string unknownErrorMessage = "Result Div not found. Please contact the dev team to update this software.";
            try
            {
                //build final search url
                string finalUrl = _scrapingService.BuildGoogleSearchUrl(baseUrl, keywords, scrapeRequest.ResultNum);

                //open browser to get full page source
                _webDriver.Navigate().GoToUrl(finalUrl);
                string pageSource = _webDriver.PageSource;
                _webDriver.Quit();
                //finalUrl = "https://www.ebay.com";
                //optional: download html to a file
                await _scrapingService.SaveToFileAsync(pageSource, keywords, DateTime.Today);
                logger.Here().Information("Saved page source to file successfully");

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(pageSource);

                var rsoDiv = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("id", "") == "rso").ToList();

                if (rsoDiv.Count == 0)
                {
                    //TODO: this should be 500 error
                    return ErrorResponseBadRequest(logger.Here(), unknownErrorMessage, ErrorType.Unknown);
                }

                var searchResultComponentDivs = rsoDiv[0].Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "") == "bkWMgd")
                    .Where(node => !node.Descendants("div").Any(childNode => childNode.GetAttributeValue("class", "") == "g kno-kp mnr-c g-blk"))
                    .ToList();
                if (searchResultComponentDivs.Count == 0)
                {
                    //TODO: this should be 500 error
                    return ErrorResponseBadRequest(logger.Here(), unknownErrorMessage, ErrorType.Unknown);
                }
                var searchResultDivs = new List<HtmlNode>();
                foreach (var searchResultComponentDiv in searchResultComponentDivs)
                {
                    var searchResultDivList = searchResultComponentDiv.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "") == "rc")
                        .ToList();
                    if (searchResultDivList.Count == 0)
                    {
                        searchResultDivs.Add(searchResultComponentDiv);  //anyway, vedio, img block count for 1 result
                    }
                    else
                    {
                        searchResultDivs.AddRange(searchResultDivList);
                    }
                }
                List<GoogleSearchResult> googleSearchResults = new List<GoogleSearchResult>();

                for (int i = 0; i < searchResultDivs.Count; i++)
                {
                    GoogleSearchResult googleSearchResult = new GoogleSearchResult
                    {
                        Links = new List<string>()
                    };
                    //index
                    googleSearchResult.Index = i + 1;  //start from 1
                    //titles
                    var titleDivNode = searchResultDivs[i].Descendants("h3")
                        .FirstOrDefault(node => node.GetAttributeValue("class", "") == "LC20lb");

                    if (titleDivNode != null)
                    {
                        googleSearchResult.Title = titleDivNode.InnerText;
                    }

                    //links
                    var anchorNodes = searchResultDivs[i].Descendants("a")
                        .Where(node => node.GetAttributeValue("href", "").StartsWith("http"))
                        .ToList();

                    foreach (var anchornode in anchorNodes)
                    {
                        string link = anchornode.GetAttributeValue("href", "");
                        googleSearchResult.Links.Add(link);
                    }

                    //description
                    var descriptionDivNode = searchResultDivs[i].Descendants("div")
                        .FirstOrDefault(node => node.GetAttributeValue("class", "") == "s");
                    var descriptionNode = descriptionDivNode?.Descendants("span")
                        .FirstOrDefault(node => node.GetAttributeValue("class", "") == "st");
                    if (descriptionNode != null)
                    {
                        googleSearchResult.Description = descriptionNode.InnerText;
                    }
                    googleSearchResults.Add(googleSearchResult);
                }

                List<GoogleSearchResult> matchingGoogleSearchResults = new List<GoogleSearchResult>();
                foreach (var googleSearchResult in googleSearchResults)
                {
                    var links = googleSearchResult.Links;
                    bool shouldContinue = false;
                    foreach (var link in links)
                    {
                        if (link != null && link.ToLowerInvariant().Contains(matchingKeywords.ToLowerInvariant()))
                        {
                            matchingGoogleSearchResults.Add(googleSearchResult);
                            //here we break the current loop then continue
                            shouldContinue = true;
                            break;
                        }
                    }
                    if (shouldContinue)
                    {
                        continue;
                    }
                    if (googleSearchResult.Title != null && googleSearchResult.Title.ToLowerInvariant().Contains(matchingKeywords.ToLowerInvariant()))
                    {
                        matchingGoogleSearchResults.Add(googleSearchResult);
                        continue;
                    }
                    if (googleSearchResult.Description != null && googleSearchResult.Description.ToLowerInvariant().Contains(matchingKeywords.ToLowerInvariant()))
                    {
                        matchingGoogleSearchResults.Add(googleSearchResult);
                        continue;
                    }
                }

                //init response
                ScrapeResponse scrapeResponse = new ScrapeResponse
                {
                    MatchingResults = matchingGoogleSearchResults,
                    MatchingPositions = _scrapingService.GetPositionsFromMatchingResults(matchingGoogleSearchResults),
                    Message = "Scraped search result successfully!"
                };
                return Ok(scrapeResponse);
            }
            catch (Exception ex)
            {
                //TODO: this should be 500 error
                return ErrorResponseBadRequest(logger.Here(), ex.Message, ErrorType.ApiExpection);
            }
        }
    }
}