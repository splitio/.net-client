using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Parsing
{
    public abstract class Matcher
    {
        //TODO: derivated to be implemented 
        public abstract bool Match(string key);
    }
}
