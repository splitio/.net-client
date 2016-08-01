using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetSDK.CommonLibraries
{
    public class SdkApiClient : ISdkApiClient
    {
        private HttpClient httpClient;

        public SdkApiClient (HTTPHeader header, string baseUrl, long connectionTimeOut, long readTimeout)
        {
            if (header.encoding == "gzip")
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                httpClient = new HttpClient(handler);
            }
            else
            {
                httpClient = new HttpClient();
            }

            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", header.authorizationApiKey);
            httpClient.DefaultRequestHeaders.Add("SplitSDKVersion", header.splitSDKVersion);
            httpClient.DefaultRequestHeaders.Add("SplitSDKSpecVersion", header.splitSDKSpecVersion);
            httpClient.DefaultRequestHeaders.Add("SplitSDKMachineName", header.splitSDKMachineName);
            httpClient.DefaultRequestHeaders.Add("SplitSDKMachineIP", header.splitSDKMachineIP);
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", header.encoding);
            httpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //TODO: find a way to store it in sepparated parameters
            httpClient.Timeout = new TimeSpan((connectionTimeOut + readTimeout)*1000); 

          
        }

        public virtual HTTPResult ExecuteGet(string requestUri)
        {
            var task = httpClient.GetAsync(requestUri);
            task.Wait();

            var response = task.Result;

            var result = new HTTPResult();
            result.statusCode = response.StatusCode;
            result.content = response.Content.ReadAsStringAsync().Result;
            return result;
        }


    }
}
