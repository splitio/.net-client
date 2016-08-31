using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Parsing
{
    public interface IMatcher
    {
        bool Match(string key);
    }
}
