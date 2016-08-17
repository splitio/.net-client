using log4net;
using NetSDK.Services.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetSDK.Domain
{
    public class ParsedSplit 
    {
        public string name { get; set; }
        public int seed { get; set; }
        public bool killed { get; set; }
        public string defaultTreatment { get; set; }
        public List<ConditionWithLogic> conditions { get; set; }
    }
}
