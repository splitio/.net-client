using Splitio.Domain;
using System.Collections.Generic;

namespace Splitio.Services.Cache.Interfaces
{
    public interface ISplitCache
    {
        void AddSplit(string splitName, ParsedSplit split);

        bool RemoveSplit(string splitName);

        void SetChangeNumber(long changeNumber);

        long GetChangeNumber();

        ParsedSplit GetSplit(string splitName);

        List<ParsedSplit> GetAllSplits();
    }
}
