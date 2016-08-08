using log4net;
using NetSDK.Domain;
using NetSDK.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Parsing
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
                    conditions = new List<ConditionWithLogic>()
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
            foreach (ConditionDefinition condition in split.conditions)
            {
                var parsedPartitions = condition.partitions;
                var combiningMatcher = ParseMatcherGroup(parsedSplit, condition.matcherGroup);
                var conditionWithLogic = new ConditionWithLogic()
                {
                    partitions = parsedPartitions,
                    matcher = combiningMatcher
                };
                parsedSplit.conditions.Add(conditionWithLogic);
            }
            return parsedSplit;
        }

        private CombiningMatcher ParseMatcherGroup(ParsedSplit parsedSplit, MatcherGroupDefinition matcherGroupDefinition)
        {
            if (matcherGroupDefinition.matchers == null || matcherGroupDefinition.matchers.Count() == 0)
            {
                throw new Exception("Missing or empty matchers");
            }

            List<AttributeMatcher> delegates = new List<AttributeMatcher>();
            foreach (MatcherDefinition matcher in matcherGroupDefinition.matchers)
            {
                delegates.Add(ParseMatcher(parsedSplit, matcher));
            }

            var combiner = ParseCombiner(matcherGroupDefinition.combiner);

            return new CombiningMatcher()
            {
                delegates = delegates,
                combiner = combiner
            };
        }

        private AttributeMatcher ParseMatcher(ParsedSplit parsedSplit, MatcherDefinition matcherDefinition)
        {
            if (matcherDefinition.matcherType == null)
            {
                throw new Exception("Missing matcher type value");
            }
            var matcherType = matcherDefinition.matcherType;
            
            Matcher matcher = null;
            try
            {              
                switch (matcherType)
                {
                    //TODO: pending implementation for concrete Matcher classes
                    case MatcherTypeEnum.ALL_KEYS: matcher = GetAllKeysMatcher(); break;
                    case MatcherTypeEnum.BETWEEN: matcher = GetBetweenMatcher(matcherDefinition); break;
                    case MatcherTypeEnum.EQUAL_TO: matcher = GetEqualToMatcher(matcherDefinition); break;
                    case MatcherTypeEnum.GREATER_THAN_OR_EQUAL_TO: matcher = GetGreaterThanOrEqualToMatcher(matcherDefinition); break;
                    case MatcherTypeEnum.IN_SEGMENT: matcher =  GetInSegmentMatcher(matcherDefinition); break;
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
            }; ;
            
            if (matcherDefinition.keySelector != null && matcherDefinition.keySelector.attribute != null)
            {
                attributeMatcher.attribute = matcherDefinition.keySelector.attribute;
            }
            
            return attributeMatcher;
        }

        private Matcher GetBetweenMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.betweenMatcherData;
            return new BetweenMatcher(matcherData.dataType, matcherData.start, matcherData.end);
        }

        private Matcher GetLessThanOrEqualToMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.unaryNumericMatcherData;
            return new LessOrEqualToMatcher(matcherData.dataType, matcherData.value);
        }

        private Matcher GetGreaterThanOrEqualToMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.unaryNumericMatcherData;
            return new GreaterOrEqualToMatcher(matcherData.dataType, matcherData.value);
        }

        private Matcher GetEqualToMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.unaryNumericMatcherData;
            return new EqualToMatcher(matcherData.dataType, matcherData.value);
        }

        private Matcher GetWhitelistMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.whitelistMatcherData;
            return new WhitelistMatcher(matcherData.whitelist);
        }

        private Matcher GetInSegmentMatcher(MatcherDefinition matcherDefinition)
        {
            var matcherData = matcherDefinition.userDefinedSegmentMatcherData;
            var segment = segmentFetcher.Fetch(matcherData.segmentName);
            return new UserDefinedSegmentMatcher(segment);
        }

        private Matcher GetAllKeysMatcher()
        {
            return new AllKeysMatcher();
        }

        private CombinerEnum ParseCombiner(CombinerEnum combinerEnum)
        {
            //TODO: this should return AndCombiner() instance
            return combinerEnum;
        }
    }
}
