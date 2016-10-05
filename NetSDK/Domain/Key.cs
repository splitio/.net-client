using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Domain
{
    public class Key
    {
        public string matchingKey { get; set; }
        public string bucketingKey { get; set; }

        public Key(string matchingKey, string bucketingKey)
        {
            this.matchingKey = matchingKey;
            this.bucketingKey = bucketingKey ?? matchingKey;
        }
    }
}
