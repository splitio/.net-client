using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Domain
{
    public enum MatcherTypeEnum
    {
        ALL_KEYS,
        IN_SEGMENT,
        WHITELIST,
        EQUAL_TO,
        GREATER_THAN_OR_EQUAL_TO,
        LESS_THAN_OR_EQUAL_TO,
        BETWEEN 
    }
}
