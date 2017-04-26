using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Parsing
{
    public class PartOfSetMatcher : IMatcher
    {
        private HashSet<string> itemsToCompare = new HashSet<string>();

        public PartOfSetMatcher(List<string> compareTo)
        {
            if (compareTo != null)
            {
                itemsToCompare.UnionWith(compareTo);
            }
        }

        public bool Match(List<string> key)
        {
            if (key == null)
            {
                return false;
            }
            
            return key.All(k => itemsToCompare.Contains(k));
        }

        public bool Match(string key)
        {
            return false;
        }

        public bool Match(DateTime key)
        {
            return false;
        }

        public bool Match(long key)
        {
            return false;
        }
    }
}
