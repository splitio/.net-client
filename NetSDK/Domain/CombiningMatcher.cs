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
        
        public bool Match(string key, Dictionary<string, object> attributes)
        {
            if (delegates == null || delegates.Count() == 0)
            {
                return false;
            }

            switch (combiner)
            {
                case CombinerEnum.AND:
                default:
                    bool result = true;
                    
                    foreach(AttributeMatcher matcher in delegates)
                    {
                        if (matcher.negate)
                        {
                            result &= !matcher.Match(key, attributes);
                        }
                        else
                        {
                            result &= matcher.Match(key, attributes);
                        }
                        
                    }

                    return result;
            }
        }
    }
}
