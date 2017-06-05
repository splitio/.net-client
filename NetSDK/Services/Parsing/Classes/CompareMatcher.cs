using Splitio.Domain;
using System;
using Splitio.CommonLibraries;
using System.Collections.Generic;
using Splitio.Services.Client.Interfaces;

namespace Splitio.Services.Parsing
{
    public abstract class CompareMatcher:IMatcher
    {
        protected DataTypeEnum? dataType;
        protected long value;
        protected long start;
        protected long end;
        public bool Match(string key, ISplitClient splitClient = null)
        {
            switch (dataType)
            {
                case DataTypeEnum.DATETIME:
                    var date = key.ToDateTime();
                    return date != null ? Match(date.Value, splitClient) : false;
                case DataTypeEnum.NUMBER:
                    long number;
                    var result = long.TryParse(key, out number);
                    return result ? Match(number, splitClient) : false;
                default: return false;
            }
        }

        public abstract bool Match(DateTime key, ISplitClient splitClient = null);

        public abstract bool Match(long key, ISplitClient splitClient = null);

        public bool Match(List<string> key, ISplitClient splitClient = null)
        {
            return false;
        }
    }
}
