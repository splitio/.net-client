using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class SegmentChange
    {
        public string name { get; set; }
        public long since { get; set; }
        public long till { get; set; }
        public List<string> added { get; set; }
        public List<string> removed { get; set; }
    }
}
