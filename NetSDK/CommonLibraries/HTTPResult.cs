using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetSDK.CommonLibraries
{
    public class HTTPResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }
    }
}
