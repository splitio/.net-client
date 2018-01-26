﻿using Splitio.Domain;
using System;
using Splitio.CommonLibraries;
using Splitio.Services.Client.Interfaces;
using System.Collections.Generic;

namespace Splitio.Services.Parsing
{
    public class EqualToMatcher : CompareMatcher, IMatcher
    {
        public EqualToMatcher(DataTypeEnum? dataType, long value)
        {
            this.dataType = dataType;
            this.value = value;
        }

        public override bool Match(long key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            if (dataType == DataTypeEnum.DATETIME)
            {
                return Match(value.ToDateTime(), attributes, splitClient);
            }

            return value == key;
        }

        public override bool Match(DateTime key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            var date = value.ToDateTime();

            return date.Date == key.Date; // Compare just date part
        }

        public override bool Match(bool key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }
    }
}
