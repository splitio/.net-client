using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class MatcherGroupDefinition
    {
        public CombinerEnum Combiner { get; set; }
        public MatcherDefinition Matchers {get; set;}
    }
}
