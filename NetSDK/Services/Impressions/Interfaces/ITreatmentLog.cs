using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Impressions.Interfaces
{
    public interface ITreatmentLog
    {
        void Log(string id, string feature, string treatment, long time);
        void Log(Key key, string feature, string treatment, long time);

    }
}
