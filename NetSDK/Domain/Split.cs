using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSDK.Domain
{
    public class Split
    {
        public string Name { get; set; }
        public long seed { get; set; }
        public StatusEnum Status { get; set; }
        public bool Killed { get; set; }
        public string DefaultTreatment { get; set; }
        public List<ConditionDefinition> Conditions { get; set; }
        public long ChangeNumber { get; set; }
    }
}
