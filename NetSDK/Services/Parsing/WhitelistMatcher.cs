using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Parsing
{
    public class WhitelistMatcher: IMatcher
    {
        private List<string> list;

        public WhitelistMatcher(List<string> list)
        {
            // TODO: Complete member initialization
            this.list = list;
        }
        public bool Match(string key)
        {
            throw new NotImplementedException();
        }
    }
}
