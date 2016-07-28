using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSDK.CommonLibraries
{
    public class HTTPHeader
    {
        public string AuthorizationApiKey { get; set; }
        public string SplitSDKVersion { get; set; }
        public string SplitSDKSpecVersion { get; set; }
        public string SplitSDKMachineName { get; set; }
        public string SplitSDKMachineIP { get; set; }
        public string Encoding { get; set; } 
    }
}
