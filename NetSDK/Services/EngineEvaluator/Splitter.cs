using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.EngineEvaluator
{
    public class Splitter
    {
        private const string Control = "CONTROL";

        public string GetTreatment(string key, long seed, List<PartitionDefinition> partitions)
        {
            if(String.IsNullOrEmpty(key))
            {
                return Control; 
            }

            if (partitions.Count() == 1 && partitions.First().size == 100)
            {
                return partitions.First().treatment;
            }

            long bucket = Math.Abs(hash(key, seed) % 100) + 1;

            int covered = 0;
            foreach(PartitionDefinition partition in partitions)
            {
                covered += partition.size;
                if (covered >= bucket)
                {
                    return partition.treatment;
                }
            }

            return Control;
        }

        private long hash(string key, long seed)
        {
            long h = 0;
            for (int i = 0; i< key.Length; i++)
            {
                h = 31 * h + key[i];
            }
            return h ^ seed;
        }
    }
}
