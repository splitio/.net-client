using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Interfaces
{
    public interface ISplitManager
    {
        List<LightSplit> Splits();

        LightSplit Split(String featureName);
    }
}
