using Splitio.Services.EngineEvaluator;
using Splitio.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Interfaces
{
    public interface IClient
    {
        string GetTreatment(string key, string feature, Dictionary<string,object> attributes);
    }
}
