using Splitio.Domain;
using System.Collections.Generic;

namespace Splitio.Services.Client.Interfaces
{
    public interface ISplitClient
    {
        ISplitManager GetSplitManager();
        string GetTreatment(string key, string feature, Dictionary<string, object> attributes = null, bool logMetricsAndImpressions = true);
        string GetTreatment(Key key, string feature, Dictionary<string, object> attributes = null, bool logMetricsAndImpressions = true);
        Dictionary<string, string> GetTreatments(string key, List<string> features, Dictionary<string, object> attributes = null);
        Dictionary<string, string> GetTreatments(Key key, List<string> features, Dictionary<string, object> attributes = null);
    }
}
