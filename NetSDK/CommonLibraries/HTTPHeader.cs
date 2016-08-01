using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSDK.CommonLibraries
{
    public class HTTPHeader
    {
        public string authorizationApiKey { get; set; }
        public string splitSDKVersion { get; set; }
        public string splitSDKSpecVersion { get; set; }
        public string splitSDKMachineName { get; set; }
        public string splitSDKMachineIP { get; set; }
        public string encoding { get; set; } 
    }
}
