using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.SegmentFetcher.Interfaces
{
    public interface ISegmentChangeFetcher
    {
        SegmentChange Fetch(string name, long change_number);
    }
}
