using Splitio.Domain;
using System;
using System.Collections.Generic;

namespace Splitio.Services.Client.Interfaces
{
    public interface ISplitManager
    {
        List<SplitView> Splits();

        SplitView Split(String featureName);
    }
}
