using Splitio.Services.Client.Interfaces;
using System;
using System.Collections.Generic;

namespace Splitio.Services.Parsing
{
    public class WhitelistMatcher: IMatcher
    {
        private List<string> list;

        public WhitelistMatcher(List<string> list)
        {
            this.list = list ?? new List<string>();
        }
        public bool Match(string key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return list.Contains(key);
        }

        public bool Match(DateTime key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(long key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(List<string> key, Dictionary<string, object> attributes, ISplitClient splitClient = null)
        {
            return false;
        }
    }
}
