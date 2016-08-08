using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Parsing
{
    public class AllKeysMatcher : IMatcher
    {
        public bool Match(string key)
        {
            return true;
        }
    }
}
