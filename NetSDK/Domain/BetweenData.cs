using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Domain
{
    public class BetweenData
    {
        public DataTypeEnum? DataType { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
    }
}
