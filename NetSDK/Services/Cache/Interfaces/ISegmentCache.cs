using Splitio.Services.SegmentFetcher.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Cache.Interfaces
{
    public interface ISegmentCache
    {
        void RegisterSegment(string segmentName);

        void AddToSegment(string segmentName, List<string> segmentKeys);

        void RemoveFromSegment(string segmentName, List<string> segmentKeys);

        bool IsInSegment(string segmentName, string key);

        void SetChangeNumber(string segmentName, long changeNumber);

        long GetChangeNumber(string segmentName);
    }
}
