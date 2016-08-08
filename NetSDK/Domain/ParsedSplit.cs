using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class ParsedSplit
    {
        public string name { get; set; }
        public long seed { get; set; }
        public bool killed { get; set; }
        public string defaultTreatment { get; set; }
        public List<ConditionWithLogic> conditions { get; set; }
    }
}
