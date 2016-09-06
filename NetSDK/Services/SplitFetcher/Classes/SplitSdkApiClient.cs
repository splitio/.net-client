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
    public class SplitSdkApiClient : SdkApiClient, ISplitSdkApiClient
    {
        private static string SplitChangesUrlTemplate = ConfigurationManager.AppSettings["SPLIT_CHANGES_URL_TEMPLATE"];
        private static string UrlParameter_Since = ConfigurationManager.AppSettings["URL_PARAMETER_SINCE"];
        private static readonly ILog Log = LogManager.GetLogger(typeof(SplitSdkApiClient));
        private const string SplitFetcherTime = "splitChangeFetcher.time";
        private const string SplitFetcherStatus = "splitChangeFetcher.status.{0}";
        private const string SplitFetcherException = "splitChangeFetcher.exception";


        public SplitSdkApiClient(HTTPHeader header, string baseUrl, long connectionTimeOut, long readTimeout, IMetricsLog metricsLog = null) : base(header, baseUrl, connectionTimeOut, readTimeout, metricsLog) { }

        public string FetchSplitChanges(long since)
        {
            var clock = new Stopwatch();
            clock.Start();
            try
            {
                var requestUri = GetRequestUri(since);
                var response = ExecuteGet(requestUri);
                if (response.statusCode == HttpStatusCode.OK)
                {
                    if (metricsLog != null)
                    {
                        metricsLog.Time(SplitFetcherTime, clock.ElapsedMilliseconds);
                        metricsLog.Count(String.Format(SplitFetcherStatus, response.statusCode), 1);
                    }

                    return response.content;
                }
                else
                {
                    Log.Error(String.Format("Http status executing FetchSplitChanges: {0} - {1}", response.statusCode.ToString(), response.content));

                    if (metricsLog != null)
                    {
                        metricsLog.Count(String.Format(SplitFetcherStatus, response.statusCode), 1);
                    }

                    return String.Empty;
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception caught executing FetchSplitChanges", e);

                if (metricsLog != null)
                {
                    metricsLog.Count(SplitFetcherException, 1);
                }

                return String.Empty;
            }
        }

        private string GetRequestUri(long since)
        {
            return String.Concat(SplitChangesUrlTemplate, UrlParameter_Since, Uri.EscapeDataString(since.ToString()));
        }
    }
}
