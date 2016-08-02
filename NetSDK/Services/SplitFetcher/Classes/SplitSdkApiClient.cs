using NetSDK.CommonLibraries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using NetSDK.Services.SplitFetcher.Interfaces;
using log4net;

namespace NetSDK.Services.SplitFetcher
{
    public class SplitSdkApiClient : SdkApiClient, ISplitSdkApiClient
    {
        private static string SplitChangesUrlTemplate = ConfigurationManager.AppSettings["SPLIT_CHANGES_URL_TEMPLATE"];
        private static string SplitChangesUrlParameter_Since = ConfigurationManager.AppSettings["SPLIT_CHANGES_URL_PARAMETER_SINCE"];
        private static readonly ILog Log = LogManager.GetLogger(typeof(SplitSdkApiClient));

        public SplitSdkApiClient(HTTPHeader header, string baseUrl, long connectionTimeOut, long readTimeout) : base(header, baseUrl, connectionTimeOut, readTimeout) { }

        public string FetchSplitChanges(long since)
        {
            try
            {
                var requestUri = GetRequestUri(since);
                var response = ExecuteGet(requestUri);
                if (response.statusCode == HttpStatusCode.OK)
                {
                    return response.content;
                }
                else
                {
                    Log.Error(String.Format("Http status executing FetchSplitChanges: {0} - {1}", response.statusCode.ToString(), response.content));
                    return String.Empty;
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception caught executing FetchSplitChanges", e);
                return String.Empty;
            }
        }

        private string GetRequestUri(long since)
        {
            return String.Concat(SplitChangesUrlTemplate, SplitChangesUrlParameter_Since, Uri.EscapeDataString(since.ToString()));
        }
    }
}
