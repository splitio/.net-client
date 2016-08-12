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
        public IMatcher matcher { get; set; }
        public bool negate { get; set; }

        public virtual bool Match(string key, Dictionary<string, object> attributes)
        {
            if (attribute == null)
            {
                if (negate)
                {
                    return !matcher.Match(key);
                }
                return matcher.Match(key);
            }

            if (attributes == null)
            {
                return false;
            }

            var value = attributes[attribute];

            if (value == null)
            {
                return false;
            }

            if (negate)
            {
                return !matcher.Match(value.ToString());
            }
            return matcher.Match(value.ToString());
        }
    }
}
