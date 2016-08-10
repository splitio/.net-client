using log4net;
using NetSDK.Services.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetSDK.Domain
{
    public class ParsedSplit : ICloneable
    {
        public string name { get; set; }
        public long seed { get; set; }
        public bool killed { get; set; }
        public string defaultTreatment { get; set; }
        public List<ConditionWithLogic> conditions { get; set; }
        public CountdownEvent segmentsNotInitialized { get; set; }
        public bool initialized
        {
            get
            {
                var result =  segmentsNotInitialized != null && segmentsNotInitialized.CurrentCount == 0;
                return result;
            }
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
