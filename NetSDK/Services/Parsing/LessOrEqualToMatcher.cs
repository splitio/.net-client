﻿using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Parsing
{
    public class LessOrEqualToMatcher: Matcher
    {
        private DataTypeEnum? dataType;
        private long value;

        public LessOrEqualToMatcher(DataTypeEnum? dataType, long value)
        {
            // TODO: Complete member initialization
            this.dataType = dataType;
            this.value = value;
        }

        public override bool Match(string key)
        {
            throw new NotImplementedException();
        }
    }
}
