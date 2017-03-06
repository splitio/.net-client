using Splitio.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Splitio.Services.Cache.Interfaces;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class JSONFileSegmentFetcher:SegmentFetcher
    {
        List<string> added;
        public JSONFileSegmentFetcher(string filePath, ISegmentCache segmentsCache):base(segmentsCache)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                var json = File.ReadAllText(filePath);
                var segmentChangesResult = JsonConvert.DeserializeObject<SegmentChange>(json);
                added = segmentChangesResult.added;
            }
        }

        public override void InitializeSegment(string name)
        {
            if (added != null)
            {
                segmentCache.AddToSegment(name, added);
            }
        }

    }
}
