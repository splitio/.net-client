using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class ConditionWithLogic
    {
        public CombiningMatcher matcher { get; set; }
        public List<PartitionDefinition> partitions { get; set; }
    }
}
