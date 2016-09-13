using Splitio.Services.Client.Classes;
using Splitio.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Splitio.Domain
{
    public class Segment: ISegment
    {
        public string name { get; set; }
        public long changeNumber;
        protected HashSet<string> keys;
        protected SdkReadinessGates gates;

        public Segment(string name, long changeNumber = -1, HashSet<string> keys = null)
        {
            this.name = name;
            this.changeNumber = changeNumber;
            this.keys = keys ?? new HashSet<string>();
        }

        public void AddKeys(HashSet<string> keys)
        {
            this.keys.UnionWith(keys);
        }

        public void RemoveKeys(HashSet<string> keys)
        {
            this.keys.ExceptWith(keys);
        }

        public bool Contains(string key)
        {
            return keys.Contains(key);
        }
    }
}
