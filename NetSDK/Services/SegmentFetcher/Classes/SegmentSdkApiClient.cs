using Splitio.CommonLibraries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Splitio.Services.SplitFetcher.Interfaces;
using log4net;
using Splitio.Services.Metrics.Interfaces;
using System.Diagnostics;

namespace Splitio.Services.SplitFetcher
{
    public class SegmentSdkApiClient : SdkApiClient, ISegmentSdkApiClient
    {
        private static string SegmentChangesUrlTemplate = ConfigurationManager.AppSettings["SEGMENT_CHANGES_URL_TEMPLATE"];
        private static string UrlParameter_Since = ConfigurationManager.AppSettings["URL_PARAMETER_SINCE"];
        private static readonly ILog Log = LogManager.GetLogger(typeof(SegmentSdkApiClient));
        private const string SegmentFetcherTime = "segmentChangeFetcher.time";
        private const string SegmentFetcherStatus = "segmentChangeFetcher.status.{0}";
        private const string SegmentFetcherException = "segmentChangeFetcher.exception";



        public SegmentSdkApiClient(HTTPHeader header, string baseUrl, long connectionTimeOut, long readTimeout, IMetricsLog metricsLog = null) : base(header, baseUrl, connectionTimeOut, readTimeout, metricsLog) { }

        public string FetchSegmentChanges(string name, long since)
        {
            var clock = new Stopwatch();
            clock.Start();
            try
            {
                var requestUri = GetRequestUri(name, since);
                var response = ExecuteGet(requestUri);
                if (response.statusCode == HttpStatusCode.OK)
                {
                    if (metricsLog != null)
                    {
                        metricsLog.Time(SegmentFetcherTime, clock.ElapsedMilliseconds);
                        metricsLog.Count(String.Format(SegmentFetcherStatus, response.statusCode), 1);
                    }

                    return response.content;
                }
                else
                {
                    if (metricsLog != null)
                    {
                        metricsLog.Count(String.Format(SegmentFetcherStatus, response.statusCode), 1);
                    }

                    Log.Error(String.Format("Http status executing FetchSegmentChanges: {0} - {1}", response.statusCode.ToString(), response.content));

                    return String.Empty;
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception caught executing FetchSegmentChanges", e);
                
                if (metricsLog != null)
                {
                    metricsLog.Count(SegmentFetcherException, 1);
                }

                return String.Empty;
            }
        }

        private string GetRequestUri(string name, long since)
        {
            var segmentChangesUrl = SegmentChangesUrlTemplate.Replace("{segment_name}", name);
            return String.Concat(segmentChangesUrl, UrlParameter_Since, Uri.EscapeDataString(since.ToString()));
        }
    }
}
