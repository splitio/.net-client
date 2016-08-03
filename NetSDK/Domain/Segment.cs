using NetSDK.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class Segment: ISegment
    {
        public string name { get; set; }
        protected long change_number;
        protected HashSet<string> keys;

        public Segment(string name, long change_number = -1, HashSet<string> keys = null)
        {
            this.name = name;
            this.change_number = change_number;
            this.keys = keys ?? new HashSet<string>();
        }

        public bool Contains(string key)
        {
            return keys.Contains(key);
        }
    }
}
