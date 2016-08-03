using NetSDK.Domain;
using NetSDK.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.SegmentFetcher.Classes
{
    public class SegmentFetcher: ISegmentFetcher
    {
        public virtual Segment Fetch(string name)
        {
            return new Segment(name);
        }
    }
}
