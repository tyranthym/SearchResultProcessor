using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchResultProcessor.Models.Responses
{
    public interface IResponseBase
    {
        string Message { get; set; }               //response message
        //bool IsSuccessful { get; set; }
    }

    public class ResponseBase : IResponseBase
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        //[JsonProperty("success")]
        //public bool IsSuccessful { get; set; }
    }
}
