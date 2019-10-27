using Moq;
using SearchResultProcessor.Models;
using SearchResultProcessor.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SearchResultProcessor.UnitTest
{
    public class ScrapingServiceTests
    {
        [Fact]
        public void GetPositionsFromMatchingResults_NoMatchingShouldReturnMessage()
        {
            //arrange             
            IScrapingService scrapingService = new GoogleScrapingService();
            //act
            string actual = scrapingService.GetPositionsFromMatchingResults(new List<GoogleSearchResult>());
            //assert
            Assert.False(string.IsNullOrWhiteSpace(actual));
        }

        [Theory]
        [InlineData("https://www.google.com/search?q=darkest+dungeon&oq=darkest+d&aqs=chrome.1.69i57j0l5.6992j0j8&sourceid=chrome&ie=UTF-8",
            "https://www.google.com/search?ei=M_i1XaG1FbHZz7sP2OCEyAE&q=darkest++++++++++++++++++++++++++dungeon&oq=darkest++++++++++++++++++++++++++dungeon&gs_l=psy-ab.3..0i67l10.1571.2830..3267...0.2..0.178.779.0j5......0....1..gws-wiz.......0i71.dVf2Go0nhRs&ved=0ahUKEwjhmvaTnr3lAhWx7HMBHVgwARkQ4dUDCAs&uact=5",
            "darkest  dungeon ", 50)]
        public void BuildGoogleSearchUrl_ShouldWorkingWithAnyBaseUrl(string baseUrl1, string baseUrl2, string keywords, int numOfResult)
        {
            //arrange             
            IScrapingService scrapingService = new GoogleScrapingService();
            //act
            string final1 = scrapingService.BuildGoogleSearchUrl(baseUrl1, keywords, numOfResult);
            string final2 = scrapingService.BuildGoogleSearchUrl(baseUrl2, keywords, numOfResult);

            //assert
            Assert.True(final1 == final2);
        }
    }
}
