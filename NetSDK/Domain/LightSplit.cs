using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Domain
{
    public class SplitView
    {
        public string name { get; set; }
        public string trafficType { get; set; }
        public bool killed { get; set; }
        public List<string> treatments { get; set; }
        public long changeNumber { get; set; }
    }
}
