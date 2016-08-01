using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class MatcherGroupDefinition
    {
        public CombinerEnum combiner { get; set; }
        public List<MatcherDefinition> matchers {get; set;}
    }
}
