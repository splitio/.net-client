﻿using Common.Logging;
using Splitio.CommonLibraries;
using Splitio.Domain;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.Client.Interfaces;
using Splitio.Services.EngineEvaluator;
using Splitio.Services.InputValidation.Classes;
using Splitio.Services.InputValidation.Interfaces;
using Splitio.Services.Metrics.Interfaces;
using Splitio.Services.Shared.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Splitio.Services.Client.Classes
{
    public abstract class SplitClient : ISplitClient
    {
        protected readonly ILog _log;
        protected readonly IKeyValidator _keyValidator;
        protected readonly ISplitNameValidator _splitNameValidator;
        protected readonly IEventTypeValidator _eventTypeValidator;
        protected readonly ITrafficTypeValidator _trafficTypeValidator;
        protected const string Control = "control";
        protected const string SdkGetTreatment = "sdk.getTreatment";
        protected const string LabelKilled = "killed";
        protected const string LabelDefaultRule = "default rule";
        protected const string LabelSplitNotFound = "definition not found";
        protected const string LabelException = "exception";
        protected const string LabelTrafficAllocationFailed = "not in split";

        protected static bool LabelsEnabled;
        protected static bool Destroyed;

        protected Splitter splitter;
        protected IListener<KeyImpression> impressionListener;
        protected IListener<Event> eventListener;
        protected IMetricsLog metricsLog;
        protected ISplitManager manager;
        protected IMetricsCache metricsCache;
        protected ISimpleCache<KeyImpression> impressionsCache;
        protected ISimpleCache<Event> eventsCache;
        protected ISplitCache splitCache;
        protected ISegmentCache segmentCache;

        private ConcurrentDictionary<string, string> treatmentCache = new ConcurrentDictionary<string, string>();

        protected IList<KeyImpression> ImpressionsQueue;

        public SplitClient(ILog log)
        {
            _log = log;
            _keyValidator = new KeyValidator(_log);
            _splitNameValidator = new SplitNameValidator(_log);
            _eventTypeValidator = new EventTypeValidator(_log);
            _trafficTypeValidator = new TrafficTypeValidator(_log);
        }

        public ISplitManager GetSplitManager()
        {
            return manager;
        }

        #region Public Methods
        public string GetTreatment(string key, string feature, Dictionary<string, object> attributes = null, bool logMetricsAndImpressions = true, bool multiple = false)
        {
            return GetTreatment(new Key(key, null), feature, attributes, logMetricsAndImpressions, multiple);
        }

        public string GetTreatment(Key key, string feature, Dictionary<string, object> attributes = null, bool logMetricsAndImpressions = true, bool multiple = false)
        {
            ImpressionsQueue = new List<KeyImpression>();

            CheckClientStatus();

            if (!_keyValidator.IsValid(key, nameof(GetTreatment))) return Control;

            var splitNameResult = _splitNameValidator.SplitNameIsValid(feature, nameof(GetTreatment));

            if (!splitNameResult.Success) return Control;

            feature = splitNameResult.Value;

            var start = CurrentTimeHelper.CurrentTimeMillis();
            var clock = new Stopwatch();
            clock.Start();

            var result = DoGetTreatment(key, feature, attributes, multiple);

            if (logMetricsAndImpressions)
            {
                if (metricsLog != null)
                {
                    metricsLog.Time(SdkGetTreatment, clock.ElapsedMilliseconds);
                }

                ImpressionsQueue.Add(BuildImpression(key.matchingKey, feature, result.Treatment, start, result.ChangeNumber, LabelsEnabled ? result.Label : null, key.bucketingKeyHadValue ? key.bucketingKey : null));

                ImpressionLog(impressionListener, ImpressionsQueue);
            }

            return result.Treatment;
        }
        
        public Dictionary<string, string> GetTreatments(string key, List<string> features, Dictionary<string, object> attributes = null)
        {
            var keys = new Key(key, null);

            return GetTreatments(keys, features, attributes);
        }

        public Dictionary<string, string> GetTreatments(Key key, List<string> features, Dictionary<string, object> attributes = null)
        {
            var treatmentsForFeatures = new Dictionary<string, string>();
            ImpressionsQueue = new List<KeyImpression>();

            CheckClientStatus();

            if (!_keyValidator.IsValid(key, nameof(GetTreatments)))
            {
                treatmentsForFeatures.Add(features.First(), Control);
            }
            else
            {
                features = _splitNameValidator.SplitNamesAreValid(features, nameof(GetTreatments));

                var start = CurrentTimeHelper.CurrentTimeMillis();
                var clock = new Stopwatch();
                clock.Start();

                foreach (var feature in features)
                {
                    var splitNameResult = _splitNameValidator.SplitNameIsValid(feature, nameof(GetTreatment));

                    if (splitNameResult.Success)
                    {
                        var treatmentResult = DoGetTreatment(key, splitNameResult.Value, attributes, true);

                        treatmentsForFeatures.Add(splitNameResult.Value, treatmentResult.Treatment);

                        ImpressionsQueue.Add(BuildImpression(key.matchingKey, splitNameResult.Value, treatmentResult.Treatment, start, treatmentResult.ChangeNumber, LabelsEnabled ? treatmentResult.Label : null, key.bucketingKeyHadValue ? key.bucketingKey : null));
                    }
                }

                if (metricsLog != null)
                {
                    metricsLog.Time(SdkGetTreatment, clock.ElapsedMilliseconds);
                }

                ImpressionLog(impressionListener, ImpressionsQueue);
            }

            ClearItemsAddedToTreatmentCache(key.matchingKey);

            return treatmentsForFeatures;
        }
        
        public virtual bool Track(string key, string trafficType, string eventType, double? value = null)
        {
            CheckClientStatus();
                
            var keyResult = _keyValidator.IsValid(new Key(key, null), nameof(Track));
            var trafficTypeResult = _trafficTypeValidator.IsValid(trafficType, nameof(trafficType));
            var eventTypeResult = _eventTypeValidator.IsValid(eventType, nameof(eventType));

            if (!keyResult || !trafficTypeResult.Success || !eventTypeResult) return false;

            try
            {               
                eventListener.Log(new Event
                {
                    key = key,
                    trafficTypeName = trafficTypeResult.Value,
                    eventTypeId = eventType,
                    value = value,
                    timestamp = CurrentTimeHelper.CurrentTimeMillis()
                });

                return true;
            }
            catch (Exception e)
            {
                _log.Error("Exception caught trying to track an event", e);
                return false;
            }
        }
        
        public abstract void Destroy();
        #endregion

        #region Protected Methods
        protected virtual TreatmentResult GetTreatmentForFeature(Key key, string feature, Dictionary<string, object> attributes)
        {
            try
            {
                var split = (ParsedSplit)splitCache.GetSplit(feature);

                if (split == null)
                {
                    _log.Warn($"Unknown or invalid feature: {feature}");

                    return new TreatmentResult(LabelSplitNotFound, Control, null);
                }

                return GetTreatment(key, split, attributes, this);
            }
            catch (Exception e)
            {
                _log.Error($"Exception caught getting treatment for feature: {feature}", e);

                return new TreatmentResult(LabelException, Control, null);
            }
        }

        protected TreatmentResult GetTreatment(Key key, ParsedSplit split, Dictionary<string, object> attributes, ISplitClient splitClient)
        {
            if (!split.killed)
            {
                bool inRollout = false;
                // use the first matching condition
                foreach (var condition in split.conditions)
                {
                    if (!inRollout && condition.conditionType == ConditionType.ROLLOUT)
                    {
                        if (split.trafficAllocation < 100)
                        {
                            // bucket ranges from 1-100.
                            var bucket = split.algo == AlgorithmEnum.LegacyHash ? splitter.LegacyBucket(key.bucketingKey, split.trafficAllocationSeed) : splitter.Bucket(key.bucketingKey, split.trafficAllocationSeed);

                            if (bucket > split.trafficAllocation)
                            {
                                return new TreatmentResult(LabelTrafficAllocationFailed, split.defaultTreatment, split.changeNumber);
                            }
                        }

                        inRollout = true;
                    }

                    var combiningMatcher = condition.matcher;
                    if (combiningMatcher.Match(key, attributes, splitClient))
                    {
                        var treatment = splitter.GetTreatment(key.bucketingKey, split.seed, condition.partitions, split.algo);

                        return new TreatmentResult(condition.label, treatment, split.changeNumber);
                    }
                }

                return new TreatmentResult(LabelDefaultRule, split.defaultTreatment, split.changeNumber);
            }
            else
            {
                return new TreatmentResult(LabelKilled, split.defaultTreatment, split.changeNumber);
            }
        }

        protected virtual void ImpressionLog<T>(IListener<T> listener, IList<T> impressions)
        {
            if (listener != null)
            {
                foreach (var imp in impressions)
                {
                    listener.Log(imp);
                }
            }
        }
        #endregion

        #region Private Methods
        private TreatmentResult DoGetTreatment(Key key, string feature, Dictionary<string, object> attributes = null, bool multiple = false)
        {
            if (feature == null)
            {
                _log.Error($"{nameof(GetTreatment)}: split_name cannot be null");

                return new TreatmentResult(string.Empty, Control, null);
            }

            var featureHash = string.Concat(key.matchingKey, "#", feature, "#", attributes != null ? attributes.GetHashCode() : 0);

            if (multiple && treatmentCache.ContainsKey(featureHash))
            {
                return new TreatmentResult(string.Empty, treatmentCache[featureHash], null);
            }

            var result = GetTreatmentForFeature(key, feature, attributes);

            if (multiple)
            {
                treatmentCache.TryAdd(featureHash, result.Treatment);
            }

            return result;
        }

        private KeyImpression BuildImpression(string matchingKey, string feature, string treatment, long time, long? changeNumber, string label, string bucketingKey)
        {
            return new KeyImpression { feature = feature, keyName = matchingKey, treatment = treatment, time = time, changeNumber = changeNumber, label = label, bucketingKey = bucketingKey };
        }

        private void ClearItemsAddedToTreatmentCache(string key)
        {
            var temporaryTreatmentCache = new ConcurrentDictionary<string, string>(treatmentCache);
            foreach (var item in temporaryTreatmentCache.Keys.Where(x => x.StartsWith(key)))
            {
                temporaryTreatmentCache.TryRemove(item, out string result);
            }

            treatmentCache = temporaryTreatmentCache;
        }

        private void CheckClientStatus()
        {
            if (Destroyed)
            {
                _log.Error("Client has already been destroyed - no calls possible");
            }
        }
        #endregion
    }
}
