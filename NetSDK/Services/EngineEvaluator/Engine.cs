using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.EngineEvaluator
{
    public class Engine
    {
        private Splitter splitter;

        public Engine(Splitter splitter)
        {
            this.splitter = splitter;
        }

        public string GetTreatment(string key, ParsedSplit split, Dictionary<string, object> attributes)
        {
            if (split.killed)
            {
                return split.defaultTreatment;
            }

            foreach(ConditionWithLogic condition in split.conditions)
            {
                var combiningMatcher = condition.matcher;
                if(combiningMatcher.Match(key, attributes))
                {
                    return splitter.GetTreatment(key, split.seed, condition.partitions);
                }
            }

            return split.defaultTreatment;
        }
    }
}
