using Common.Logging;
using Splitio.Domain;
using Splitio.Services.SegmentFetcher.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public abstract class SegmentChangeFetcher: ISegmentChangeFetcher
    {
        private SegmentChange segmentChange;
        private static readonly ILog Log = LogManager.GetLogger(typeof(SegmentChangeFetcher));

        protected abstract Task<SegmentChange> FetchFromBackend(string name, long since, CancellationToken token = default(CancellationToken));

        public async Task<SegmentChange> Fetch(string name, long since, CancellationToken token = default(CancellationToken))
        {
            try
            {
                segmentChange = await FetchFromBackend(name, since, token);
            }
            catch(Exception e)
            {
                Log.Error(String.Format("Exception caught executing fetch segment changes since={0}", since), e);
                segmentChange = null; 
            }                   
            return segmentChange;
        }
    }   
}
