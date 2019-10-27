using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchResultProcessor.Models
{
    public interface IGoogleSearchResult
    {
        string Description { get; set; }
        int Index { get; set; }
        List<string> Links { get; set; }
        string Title { get; set; }
    }

    public class GoogleSearchResult : IGoogleSearchResult
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("links")]
        public List<string> Links { get; set; }   //all <a> links within individual search result which start with "http"
        [JsonProperty("description")]
        public string Description { get; set; }   //brief intro
        [JsonProperty("index")]
        public int Index { get; set; }            //searching position (start from 1)
    }
}
