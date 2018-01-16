
using System.Diagnostics;
using System.Reflection;

namespace Splitio
{
    public static class Version
    {
        public static string SplitSdkVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        public static string SplitSpecVersion = "1.0";
    }
}
