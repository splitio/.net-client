using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Parsing
{
    public class WhitelistMatcher: IMatcher
    {
        private List<string> list;

        public WhitelistMatcher(List<string> list)
        {
            this.list = list ?? new List<string>();
        }
        public bool Match(string key)
        {
            return list.Contains(key);
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
