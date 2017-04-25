using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Parsing.Classes
{
    public class ContainsAllOfSetMatcher : IMatcher
    {
        private HashSet<string> itemsToCompare = new HashSet<string>();

        public ContainsAllOfSetMatcher(List<string> compareTo)
        {
            if (compareTo == null)
            {
                throw new ArgumentNullException("ContainsAllOfSetMatcher does not allow null list as input.");
            }

            itemsToCompare.UnionWith(compareTo);
        }

        public bool Match(List<string> key)
        {
            if (key == null)
            {
                return false;
            }
            
            return itemsToCompare.All(i => key.Contains(i));
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
