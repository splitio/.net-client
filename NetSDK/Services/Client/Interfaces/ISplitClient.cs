using Splitio.Domain;
using Splitio.Services.EngineEvaluator;
using Splitio.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Interfaces
{
    public interface ISplitClient
    {
        ISplitManager GetSplitManager();
        string GetTreatment(string key, string feature, Dictionary<string, object> attributes = null);
        string GetTreatment(Key key, string feature, Dictionary<string, object> attributes = null);

    }
}
