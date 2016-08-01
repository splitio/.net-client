using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class SplitChangesResult
    {
        public long since { get; set; }
        public long till { get; set; }
        public List<Split> splits { get; set; }
    }
}
