using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.SegmentFetcher.Interfaces
{
    public interface ISegmentChangeFetcher
    {
        SegmentChange Fetch(string name, long change_number);
    }
}
