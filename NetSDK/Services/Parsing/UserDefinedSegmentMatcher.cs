﻿using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Parsing
{
    public class UserDefinedSegmentMatcher: IMatcher
    {
        private Segment segment;

        public UserDefinedSegmentMatcher(Segment segment)
        {
            // TODO: Complete member initialization
            this.segment = segment;
        }
        public bool Match(string key)
        {
            throw new NotImplementedException();
        }
    }
}