using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Splitio.Services.Parsing
{
    public class MatchesStringMatcher: BaseMatcher, IMatcher
    {
        Regex regex;


        public MatchesStringMatcher(string pattern)
        {
            regex = new Regex(pattern);
        }


        public override bool Match(string key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return regex.IsMatch(key);
        }

        public override bool Match(Key key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return regex.IsMatch(key.matchingKey);
        }


        public override bool Match(DateTime key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return false;
        }

        public override bool Match(long key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return false;
        }

        public override bool Match(List<string> key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return false;
        }

        public override bool Match(bool key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return false;
        }
    }
}
