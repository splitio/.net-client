using log4net;
using NetSDK.Domain;
using NetSDK.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.SegmentFetcher.Classes
{
    public abstract class SegmentChangeFetcher: ISegmentChangeFetcher
    {
        private SegmentChange segmentChange;
        private static readonly ILog Log = LogManager.GetLogger(typeof(SegmentChangeFetcher));

        protected abstract SegmentChange FetchFromBackend(string name, long since);

        private SegmentChange BuildEmptyResponse(string name, long since)
        {
            return new SegmentChange()
            {
                since = since,
                till = since,
                name = name,
                added = new List<string>(),
                removed = new List<string>()
            };
        }

        public SegmentChange Fetch(string name, long since)
        {
            try
            {
                segmentChange = FetchFromBackend(name, since);
                if (segmentChange == null)
                {
                    segmentChange = BuildEmptyResponse(name, since);
                }
            }
            catch(Exception e)
            {
                Log.Error(String.Format("Exception caught executing fetch segment changes since={0}", since), e);
                segmentChange = BuildEmptyResponse(name, since); 
            }                   
            return segmentChange;
        }
    }   
}
