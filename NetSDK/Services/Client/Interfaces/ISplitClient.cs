using Splitio.Domain;
using System.Collections.Generic;

namespace Splitio.Services.Client.Interfaces
{
    public interface ISplitClient
    {
        ISplitManager GetSplitManager();
        string GetTreatment(string key, string feature, Dictionary<string, object> attributes = null);
        string GetTreatment(Key key, string feature, Dictionary<string, object> attributes = null);

    }
}
