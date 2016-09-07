﻿using log4net;
using Splitio.Domain;
using Splitio.Services.SegmentFetcher.Classes;
using Splitio.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Parsing
{
    public class SplitParser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SplitParser));
        private readonly ISegmentFetcher segmentFetcher;

        public SplitParser(ISegmentFetcher segmentFetcher)
        {
            this.segmentFetcher = segmentFetcher;
        }

        public ParsedSplit Parse(Split split)
        {
            try
            {
                if (split.status != StatusEnum.ACTIVE)
                {
                    return null;
                }

                ParsedSplit parsedSplit = new ParsedSplit()
                {
                    name = split.name,
                    killed = split.killed,
                    defaultTreatment = split.defaultTreatment,
                    seed = split.seed,
                    conditions = new List<ConditionWithLogic>(),
                    changeNumber = split.changeNumber,
                    trafficTypeName = split.trafficTypeName
                };
                parsedSplit = ParseConditions(split, parsedSplit);
                return parsedSplit;
            }
            catch (Exception e)
            {
                Log.Error("Exception caught parsing split", e);
                return null;
            }
        }

        private ParsedSplit ParseConditions(Split split, ParsedSplit parsedSplit)
        {
            parsedSplit.conditions.AddRange(split.conditions.Select(x => new ConditionWithLogic()
            {
                partitions = x.partitions,
                matcher = ParseMatcherGroup(parsedSplit, x.matcherGroup)
            }));

            return parsedSplit;
        }

        private CombiningMatcher ParseMatcherGroup(ParsedSplit parsedSplit, MatcherGroupDefinition matcherGroupDefinition)
        {
            if (matcherGroupDefinition.matchers == null || matcherGroupDefinition.matchers.Count() == 0)
            {
                throw new Exception("Missing or empty matchers");
            }

            return new CombiningMatcher()
            {
                delegates = matcherGroupDefinition.matchers.Select(x => ParseMatcher(parsedSplit, x)).ToList(),
                combiner = ParseCombiner(matcherGroupDefinition.combiner)
            };
        }

        private AttributeMatcher ParseMatcher(ParsedSplit parsedSplit, MatcherDefinition matcherDefinition)
        {
            if (matcherDefinition.matcherType == null)
            {
                throw new Exception("Missing matcher type value");
            }
            var matcherType = matcherDefinition.matcherType;

            IMatcher matcher = null;
            try
            {              
                switch (matcherType)
                {
                    case MatcherTypeEnum.ALL_KEYS: matcher = GetAllKeysMatcher(); break;
                    case MatcherTypeEnum.BETWEEN: matcher = GetBetweenMatcher(matcherDefinition); break;
                    case MatcherTypeEnum.EQUAL_TO: matcher = GetEqualToMatcher(matcherDefinition); break;
                    case MatcherTypeEnum.GREATER_THAN_OR_EQUAL_TO: matcher = GetGreaterThanOrEqualToMatcher(matcherDefinition); break;
                    case MatcherTypeEnum.IN_SEGMENT: matcher = GetInSegmentMatcher(matcherDefinition, parsedSplit); break;                    
                    case MatcherTypeEnum.LESS_THAN_OR_EQUAL_TO: matcher = GetLessThanOrEqualToMatcher(matcherDefinition); break;
                    case MatcherTypeEnum.WHITELIST: matcher = GetWhitelistMatcher(matcherDefinition); break;
                }
            }
            catch(Exception e)
            {
                Log.Error("Error parsing matcher", e);
            }

            if (matcher == null)
            {
                throw new Exception(String.Format("Unable to create matcher for matcher type: {0}", matcherType));
            }

            AttributeMatcher attributeMatcher = new AttributeMatcher()
            {              
                matcher = matcher,
                negate = matcherDefinition.negate
            }; 
            
            if (matcherDefinition.keySelector != null && matcherDefinition.keySelector.attribute != null)
            {
                attributeMatcher.attribute = matcherDefinition.keySelector.attribute;
            }
            
            return attributeMatcher;
        }

        private IMatcher GetBetweenMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.betweenMatcherData;
            return new BetweenMatcher(matcherData.dataType, matcherData.start, matcherData.end);
        }

        private IMatcher GetLessThanOrEqualToMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.unaryNumericMatcherData;
            return new LessOrEqualToMatcher(matcherData.dataType, matcherData.value);
        }

        private IMatcher GetGreaterThanOrEqualToMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.unaryNumericMatcherData;
            return new GreaterOrEqualToMatcher(matcherData.dataType, matcherData.value);
        }

        private IMatcher GetEqualToMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.unaryNumericMatcherData;
            return new EqualToMatcher(matcherData.dataType, matcherData.value);
        }

        private IMatcher GetWhitelistMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.whitelistMatcherData;
            return new WhitelistMatcher(matcherData.whitelist);
        }

        private IMatcher GetInSegmentMatcher(MatcherDefinition matcherDefinition, ParsedSplit parsedSplit)
        {
            var matcherData = matcherDefinition.userDefinedSegmentMatcherData;
            var segment = segmentFetcher.Fetch(matcherData.segmentName);
            return new UserDefinedSegmentMatcher(segment);
        }

        private IMatcher GetAllKeysMatcher()
        {
            return new AllKeysMatcher();
        }

        private CombinerEnum ParseCombiner(CombinerEnum combinerEnum)
        {
            return combinerEnum;
        }
    }
}
