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

namespace Splitio.Services.SplitFetcher
{
    public class SegmentSdkApiClient : SdkApiClient, ISegmentSdkApiClient
    {
        private static string SegmentChangesUrlTemplate = ConfigurationManager.AppSettings["SEGMENT_CHANGES_URL_TEMPLATE"];
        private static string UrlParameter_Since = ConfigurationManager.AppSettings["URL_PARAMETER_SINCE"];
        private static readonly ILog Log = LogManager.GetLogger(typeof(SegmentSdkApiClient));

        public SegmentSdkApiClient(HTTPHeader header, string baseUrl, long connectionTimeOut, long readTimeout) : base(header, baseUrl, connectionTimeOut, readTimeout) { }

        public string FetchSegmentChanges(string name, long since)
        {
            try
            {
                var requestUri = GetRequestUri(name, since);
                var response = ExecuteGet(requestUri);
                if (response.statusCode == HttpStatusCode.OK)
                {
                    return response.content;
                }
                else
                {
                    Log.Error(String.Format("Http status executing FetchSegmentChanges: {0} - {1}", response.statusCode.ToString(), response.content));
                    return String.Empty;
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception caught executing FetchSegmentChanges", e);
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
