﻿using Splitio.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class JSONFileSegmentFetcher:SegmentFetcher
    {
        List<string> added;
        public JSONFileSegmentFetcher(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var segmentChangesResult = JsonConvert.DeserializeObject<SegmentChange>(json);
            added = segmentChangesResult.added;
        }

        public override Segment Fetch(string name)
        {
            var segment = new Segment(name, keys : new HashSet<string>(added));
            return segment;
        }

    }
}
