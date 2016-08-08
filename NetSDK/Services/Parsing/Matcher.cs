using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Parsing
{
    public interface IMatcher
    {
        //TODO: derivated to be implemented 
        bool Match(string key);
    }
}
