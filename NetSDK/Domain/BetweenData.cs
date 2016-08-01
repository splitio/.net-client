using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class BetweenData
    {
        public DataTypeEnum? dataType { get; set; }
        public long start { get; set; }
        public long end { get; set; }
    }
}
