using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.SegmentFetcher.Interfaces
{
    public interface ISegmentFetcher
    {
        Segment Fetch(string name);
    }
}
