using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Splitio.CommonLibraries
{
    public class HTTPResult
    {
        public HttpStatusCode statusCode { get; set; }
        public string content { get; set; }
    }
}
