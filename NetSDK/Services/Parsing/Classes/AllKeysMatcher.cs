using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Parsing
{
    public class AllKeysMatcher : IMatcher
    {
        public bool Match(string key)
        {
            return key != null;
        }
    }
}
