using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.EngineEvaluator
{
    public class Engine
    {
        private Splitter splitter;

        public Engine()
        {
            this.splitter = new Splitter();
        }

        public Engine(Splitter splitter)
        {
            this.splitter = splitter ?? new Splitter();
        }

        public virtual string GetTreatment(string key, ParsedSplit split, Dictionary<string, object> attributes)
        {
            if (!split.killed)
            {
                foreach (ConditionWithLogic condition in split.conditions)
                {
                    var combiningMatcher = condition.matcher;
                    if (combiningMatcher.Match(key, attributes))
                    {
                        return splitter.GetTreatment(key, split.seed, condition.partitions);
                    }
                }
            }

            return split.defaultTreatment;
        }
    }
}
