using NetSDK.CommonLibraries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;

namespace NetSDK.Services.SplitFetcher
{
    public class SplitSdkApiClient : SdkApiClient
    {
        private string _SplitChangesUrlTemplate = ConfigurationManager.AppSettings["SPLIT_CHANGES_URL_TEMPLATE"];
        private string _SplitChangesUrlParameter_Since = ConfigurationManager.AppSettings["SPLIT_CHANGES_URL_PARAMETER_SINCE"];
        public SplitSdkApiClient(HTTPHeader header, string baseUrl, long connectionTimeOut, long readTimeout) : base(header, baseUrl, connectionTimeOut, readTimeout) { }

        public string FetchSplitChanges(string since)
        {
            try
            {
                var requestUri = GetRequestUri(since);
                var response = ExecuteGet(requestUri);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.Content;
                }
                else
                {
                    //TODO: log error
                    return String.Empty;
                }
            }
            catch (Exception e)
            {
                //TODO: log error
                return String.Empty;
            }
        }

        private string GetRequestUri(string since)
        {
            return String.Concat(_SplitChangesUrlTemplate, _SplitChangesUrlParameter_Since, Uri.EscapeDataString(since));
        }
    }
}
