using Splitio.Domain;
using Splitio.Services.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Parsing.Classes
{
    public class DependencyMatcher : IMatcher
    {
        string split { get; set; }
        List<string> treatments { get; set; }

        public DependencyMatcher(string split, List<string> treatments)
        {
            this.split = split;
            this.treatments = treatments;
        }

        public bool Match(string key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(Key key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            if (splitClient == null)
            {
                return false;
            }

            string treatment = splitClient.GetTreatment(key, split, attributes, false, false); 
            
            return treatments.Contains(treatment);
        }

        public bool Match(DateTime key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(long key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;

        }

        public bool Match(List<string> key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }
    }
}
