using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class CombiningMatcher
    {
        public CombinerEnum combiner { get; set; }
        public List<AttributeMatcher> delegates { get; set; }
    }
}
