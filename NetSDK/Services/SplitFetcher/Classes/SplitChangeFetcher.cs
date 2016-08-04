using log4net;
using NetSDK.Domain;
using NetSDK.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.SplitFetcher.Classes
{
    public abstract class SplitChangeFetcher : ISplitChangeFetcher
    {
        private SplitChangesResult splitChanges;
        private static readonly ILog Log = LogManager.GetLogger(typeof(SplitChangeFetcher));

        //TODO: add logger in constructor
        protected abstract SplitChangesResult FetchFromBackend(long since);

        private SplitChangesResult BuildEmptyResponse(long since)
        {
            return new SplitChangesResult()
            {
                since = since,
                till = since,
                splits = new List<Split>()
            };
        }

        public SplitChangesResult Fetch(long since)
        {
            try
            {
                splitChanges = FetchFromBackend(since);
                if (splitChanges == null)
                {
                    splitChanges = BuildEmptyResponse(since);
                }
            }
            catch(Exception e)
            {
                Log.Error(String.Format("Exception caught executing Fetch since={0}", since), e);
                splitChanges = BuildEmptyResponse(since); 
            }                   
            return splitChanges;
        }
    }
}
