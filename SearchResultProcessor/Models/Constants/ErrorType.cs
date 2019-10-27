using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchResultProcessor.Models.Constants
{
    public class ErrorType
    {
        public const string Unknown = "unknown";
        public const string General = "general";

        //typical 400 errors
        public const string IdNotProvided = "id_not_provided";
        public const string IdNotMatched = "id_not_matched";


        public const string BusinessRuleViolation = "violate_business_rule";
        public const string InvalidRequestInput = "invalid_request_input";
        public const string ApiExpection = "api_exception";
    }
}
