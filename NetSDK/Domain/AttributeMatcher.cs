﻿using Splitio.Services.Client.Interfaces;
using Splitio.Services.Parsing;
using System.Collections.Generic;

namespace Splitio.Domain
{
    public class AttributeMatcher
    {
        public string attribute { get; set; }
        public IMatcher matcher { get; set; }
        public bool negate { get; set; }

        public virtual bool Match(string key, Dictionary<string, object> attributes, ISplitClient splitClient = null)
        {
            if (attribute == null)
            {
                return (negate ^ matcher.Match(key, splitClient));
            }

            if (attributes == null)
            {
                return false;
            }

            object value;
            attributes.TryGetValue(attribute, out value);

            if (value == null)
            {
                return false;
            }

            try
            {
                return (negate ^ matcher.Match((dynamic)value, splitClient));
            }
            catch
            {
                return false;
            }
        }
    }
}
