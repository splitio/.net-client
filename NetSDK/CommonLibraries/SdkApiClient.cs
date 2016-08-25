﻿using log4net;
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

namespace Splitio.CommonLibraries
{
    public class SdkApiClient : ISdkApiClient
    {
        private HttpClient httpClient;
        private static readonly ILog Log = LogManager.GetLogger(typeof(SdkApiClient));

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
            if (!String.IsNullOrEmpty(header.splitSDKMachineName))
            {
                httpClient.DefaultRequestHeaders.Add("SplitSDKMachineName", header.splitSDKMachineName);
            }
            if (!String.IsNullOrEmpty(header.splitSDKMachineIP))
            {
                httpClient.DefaultRequestHeaders.Add("SplitSDKMachineIP", header.splitSDKMachineIP);
            }
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", header.encoding);
            httpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //TODO: find a way to store it in sepparated parameters
            httpClient.Timeout = TimeSpan.FromMilliseconds((connectionTimeOut + readTimeout));         
        }

        public virtual HTTPResult ExecuteGet(string requestUri)
        {
            var result = new HTTPResult();
            try
            {
                var task = httpClient.GetAsync(requestUri);
                task.Wait();
                var response = task.Result;
                result.statusCode = response.StatusCode;
                result.content = response.Content.ReadAsStringAsync().Result;                
            }
            catch(Exception e)
            {
                Log.Error(String.Format("Exception caught executing GET {0}", requestUri), e);
            }
            return result;
        }


    }
}
