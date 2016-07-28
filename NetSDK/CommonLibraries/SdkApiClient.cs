using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetSDK.CommonLibraries
{
    public class SdkApiClient
    {
        private HttpClient _httpClient { get; set; }

        public SdkApiClient (HTTPHeader header, string baseUrl, long connectionTimeOut, long readTimeout)
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", header.AuthorizationApiKey);
            _httpClient.DefaultRequestHeaders.Add("SplitSDKVersion", header.SplitSDKVersion);
            _httpClient.DefaultRequestHeaders.Add("SplitSDKSpecVersion", header.SplitSDKSpecVersion);
            _httpClient.DefaultRequestHeaders.Add("SplitSDKMachineName", header.SplitSDKMachineName);
            _httpClient.DefaultRequestHeaders.Add("SplitSDKMachineIP", header.SplitSDKMachineIP);
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", header.Encoding);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //TODO: find a way to store it in sepparated parameters
            _httpClient.Timeout = new TimeSpan((connectionTimeOut + readTimeout)*1000); 
        }

        public HTTPResult ExecuteGet(string requestUri)
        {
            var response = _httpClient.GetAsync(requestUri).Result;

            var result = new HTTPResult();

            result.StatusCode = response.StatusCode;
            result.Content = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        public async Task ExecutePost<T>(string requestUri, T data)
        {
            //TODO: pending implementation
        }
    }
}
