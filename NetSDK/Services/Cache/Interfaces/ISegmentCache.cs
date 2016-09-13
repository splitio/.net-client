using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Cache.Interfaces
{
    public interface ISegmentCache
    {
        void AddToSegment(string segmentName, HashSet<string> segmentKeys);

        void RemoveFromSegment(string segmentName, HashSet<string> segmentKeys);

        bool IsInSegment(string segmentName, string key);

        void SetChangeNumber(string segmentName, long changeNumber);

        long GetChangeNumber(string segmentName);
    }
}
