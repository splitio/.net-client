using log4net;
using Splitio.CommonLibraries;
using Splitio.Services.Impressions.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;

namespace Splitio.Services.Impressions.Classes
{
    public class TreatmentSdkApiClient : SdkApiClient, ITreatmentSdkApiClient
    {
        private static string TestImpressionsUrlTemplate = ConfigurationManager.AppSettings["TEST_IMPRESSIONS_URL_TEMPLATE"];
        private static readonly ILog Log = LogManager.GetLogger(typeof(TreatmentSdkApiClient));

        public TreatmentSdkApiClient(HTTPHeader header, string baseUrl, long connectionTimeOut, long readTimeout) : base(header, baseUrl, connectionTimeOut, readTimeout) { }

        public void SendBulkImpressions(string impressions)
        {
            var response = ExecutePost(TestImpressionsUrlTemplate, impressions);
            if (response.statusCode != HttpStatusCode.OK)
            {
                Log.Error(String.Format("Http status executing SendBulkImpressions: {0} - {1}", response.statusCode.ToString(), response.content));
            }
        }
    }
}
