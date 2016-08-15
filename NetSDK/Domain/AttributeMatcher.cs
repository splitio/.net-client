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

        //TODO: PROS AND CONS --> getkey getattributes
        //match using state (more properties) instead of passing parameters (to avoid changing match protocol if another comparison parameter is needed)
        public virtual bool Match(string key, Dictionary<string, object> attributes)
        {
            if (attribute == null)
            {
                return (negate ^ matcher.Match(key));
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

            //TODO: avoid toString and develop Match for each input type STRING, NUMBER, DATE

            return (negate ^ matcher.Match(value.ToString()));
        }
    }
}
