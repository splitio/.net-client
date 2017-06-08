using Splitio.Services.Client.Interfaces;
using System;
using System.Collections.Generic;

namespace Splitio.Services.Parsing
{
    public class AllKeysMatcher : IMatcher
    {
        public bool Match(string key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return key != null;
        }

        public bool Match(DateTime key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return key != null;
        }

        public bool Match(long key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return key != null;
        }


        public bool Match(List<string> key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }
    }
}
