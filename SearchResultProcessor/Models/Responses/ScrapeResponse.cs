using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchResultProcessor.Models.Responses
{
    public class ScrapeResponse : ResponseBase
    {
        [JsonProperty("matchingResults")]
        public List<GoogleSearchResult> MatchingResults { get; set; }

        [JsonProperty("matchingPositions")]
        public string MatchingPositions { get; set; }
    }
}
