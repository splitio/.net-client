using Splitio.Services.Client.Interfaces;
using System;
using System.Collections.Generic;

namespace Splitio.Services.Parsing
{
    public class AllKeysMatcher : IMatcher
    {
        public bool Match(string key, ISplitClient splitClient = null)
        {
            return key != null;
        }

        public bool Match(DateTime key, ISplitClient splitClient = null)
        {
            return key != null;
        }

        public bool Match(long key, ISplitClient splitClient = null)
        {
            return key != null;
        }


        public bool Match(List<string> key, ISplitClient splitClient = null)
        {
            return false;
        }
    }
}
