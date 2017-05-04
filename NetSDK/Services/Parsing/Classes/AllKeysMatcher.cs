using System;
using System.Collections.Generic;

namespace Splitio.Services.Parsing
{
    public class AllKeysMatcher : IMatcher
    {
        public bool Match(string key)
        {
            return key != null;
        }

        public bool Match(DateTime key)
        {
            return key != null;
        }

        public bool Match(long key)
        {
            return key != null;
        }


        public bool Match(List<string> key)
        {
            return false;
        }
    }
}
