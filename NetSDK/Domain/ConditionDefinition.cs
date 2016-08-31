using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Domain
{
    public class ConditionDefinition
    {
        public MatcherGroupDefinition matcherGroup { get; set; }
        public List<PartitionDefinition> partitions { get; set; }
        public string label { get; set; }
    }
}
