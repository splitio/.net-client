﻿using System.Collections.Generic;

namespace Splitio.Domain
{
    public class Split
    {
        public string name { get; set; }
        public int seed { get; set; }
        public StatusEnum status { get; set; }
        public bool killed { get; set; }
        public string defaultTreatment { get; set; }
        public List<ConditionDefinition> conditions { get; set; }
        public long changeNumber { get; set; }
        public string trafficTypeName { get; set; }

    }
}
