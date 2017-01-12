﻿using System.Collections.Generic;

namespace Splitio.Domain
{
    public class ParsedSplit 
    {
        public string name { get; set; }
        public int seed { get; set; }
        public bool killed { get; set; }
        public string defaultTreatment { get; set; }
        public List<ConditionWithLogic> conditions { get; set; }
        public long changeNumber { get; set; }
        public string trafficTypeName { get; set; }

    }
}
