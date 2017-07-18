using Splitio.Domain;
using Splitio.Services.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Parsing
{
    public class StartsWithMatcher : IMatcher
    {
        private HashSet<string> itemsToCompare = new HashSet<string>();

        public StartsWithMatcher(List<string> compareTo)
        {
            if (compareTo != null)
            {
                itemsToCompare.UnionWith(compareTo);
            }
        }


        public bool Match(string key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return itemsToCompare.Any(i => key.StartsWith(i));
        }

        public bool Match(Key key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            if (String.IsNullOrEmpty(key.matchingKey))
            {
                return false;
            }

            return itemsToCompare.Any(i => key.matchingKey.StartsWith(i));
        }

        public bool Match(List<string> key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(DateTime key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(long key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

    }
}
