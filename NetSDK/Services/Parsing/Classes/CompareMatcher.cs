using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Parsing
{
    public abstract class CompareMatcher: IMatcher
    {
        protected DataTypeEnum? dataType;
        protected long value;
        protected long start;
        protected long end;

        public bool Match(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }
            switch (dataType)
            {
                case DataTypeEnum.DATETIME:
                    return MatchDate(key);
                case DataTypeEnum.NUMBER:
                    return MatchNumber(key);
                default: return false;
            }
        }

        protected abstract bool MatchNumber(string key);
        protected abstract bool MatchDate(string key);
    }
}
