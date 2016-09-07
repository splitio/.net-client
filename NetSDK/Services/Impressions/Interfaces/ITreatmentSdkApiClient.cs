using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Impressions.Interfaces
{
    public interface ITreatmentSdkApiClient
    {
        void SendBulkImpressions(string impressions);
    }
}
