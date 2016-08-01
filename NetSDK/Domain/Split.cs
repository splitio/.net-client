using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSDK.Domain
{
    public class Split: ICloneable
    {
        public string name { get; set; }
        public long seed { get; set; }
        public StatusEnum status { get; set; }
        public bool killed { get; set; }
        public string defaultTreatment { get; set; }
        public List<ConditionDefinition> conditions { get; set; }
        public long changeNumber { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
