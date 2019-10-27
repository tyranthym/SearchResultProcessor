using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SearchResultProcessor.Models.Requests
{
    /// <summary>
    /// from query only
    /// </summary>
    public class ScrapeRequest
    {
        [BindProperty(Name = "keywords")]
        public string Keywords { get; set; }          //search input


        [BindProperty(Name = "resultNum")]
        public int ResultNum { get; set; }            //result list count

        //uncomment to enable url format validation
        //[Url]
        [BindProperty(Name = "matchingKeywords")]
        public string MatchingKeywords { get; set; }    //target keywords or url
    }

    //fluent validation validatior
    public class ScrapeRequestValidator : AbstractValidator<ScrapeRequest>
    {
        public ScrapeRequestValidator()
        {
            RuleFor(model => model.Keywords).NotEmpty().MaximumLength(80);
            RuleFor(model => model.ResultNum).NotEmpty().GreaterThan(0).LessThanOrEqualTo(200);
            RuleFor(model => model.MatchingKeywords).NotEmpty();
        }
    }

}
