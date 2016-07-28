using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class MatcherDefinition
    {
        public KeySelector KeySelector { get; set; }
        public MatcherTypeEnum MatcherType { get; set; }
        public bool Negate { get; set; }
        public UserDefinedSegmentData UserDefinedSegmentMatcherData { get; set; }
        public WhitelistData WhitelistMatcherData { get; set; }
        public UnaryNumericData UnaryNumericMatcherData { get; set; }
        public BetweenData BetweenMatcherData { get; set; }
    }
}
