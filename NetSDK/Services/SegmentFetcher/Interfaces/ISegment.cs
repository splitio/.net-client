using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.SegmentFetcher.Interfaces
{
    public interface ISegment
    {
        bool Contains(string key);
    }
}
