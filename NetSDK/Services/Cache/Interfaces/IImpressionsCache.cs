using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Cache.Interfaces
{
    public interface IImpressionsCache
    {
        void AddImpression(KeyImpression impression);

        List<KeyImpression> FetchAllAndClear();
    }
}
