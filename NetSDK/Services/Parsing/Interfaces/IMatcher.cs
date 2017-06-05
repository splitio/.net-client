using Splitio.Services.Client.Interfaces;
using System;
using System.Collections.Generic;

namespace Splitio.Services.Parsing
{
    public interface IMatcher
    {
        bool Match(string key, ISplitClient splitClient = null);

        bool Match(DateTime key, ISplitClient splitClient = null);

        bool Match(long key, ISplitClient splitClient = null);

        bool Match(List<string> key, ISplitClient splitClient = null);
    }
}
