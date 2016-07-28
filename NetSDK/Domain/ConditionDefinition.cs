using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class ConditionDefinition
    {
        public MatcherGroupDefinition MatcherGroup { get; set; }
        public List<PartitionDefinition> Partitions { get; set; }
        public string Label { get; set; }
    }
}
