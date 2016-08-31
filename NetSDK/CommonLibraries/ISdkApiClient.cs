using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.CommonLibraries
{
    public interface ISdkApiClient
    {
        HTTPResult ExecuteGet(string requestUri);
    }
}
