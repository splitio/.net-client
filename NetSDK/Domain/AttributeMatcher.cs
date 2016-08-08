using NetSDK.Services.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class AttributeMatcher
    {
        public string attribute { get; set; }
        public Matcher matcher { get; set; }
        public bool negate { get; set; }
    }
}
