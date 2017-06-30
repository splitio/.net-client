using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Splitio.Services.Parsing.Classes
{
    public class MatchesStringMatcher:IMatcher
    {
        Regex regex;


        public MatchesStringMatcher(string pattern)
        {
            regex = new Regex(pattern);
        }


        public bool Match(string key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return regex.IsMatch(key);
        }

        public bool Match(Key key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return regex.IsMatch(key.matchingKey);
        }


        public bool Match(DateTime key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(long key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(List<string> key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(bool key, Dictionary<string, object> attributes = null, Client.Interfaces.ISplitClient splitClient = null)
        {
            return false;
        }
    }
}
