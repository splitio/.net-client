using Splitio.Domain;
using System;
using Splitio.CommonLibraries;
using System.Collections.Generic;
using Splitio.Services.Client.Interfaces;

namespace Splitio.Services.Parsing
{
    public abstract class CompareMatcher: BaseMatcher, IMatcher
    {
        protected DataTypeEnum? dataType;
        protected long value;
        protected long start;
        protected long end;
        public override bool Match(string key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            switch (dataType)
            {
                case DataTypeEnum.DATETIME:
                    var date = key.ToDateTime();
                    return date != null ? Match(date.Value, attributes, splitClient) : false;
                case DataTypeEnum.NUMBER:
                    long number;
                    var result = long.TryParse(key, out number);
                    return result ? Match(number, attributes, splitClient) : false;
                default: return false;
            }
        }

        public abstract override bool Match(DateTime key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null);

        public abstract override bool Match(long key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null);

        public override bool Match(List<string> key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

        public override bool Match(Key key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return Match(key.matchingKey, attributes, splitClient);
        }
        
        public override bool Match(bool key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }
    }
}
