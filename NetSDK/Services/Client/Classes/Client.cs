using log4net;
using NetSDK.Services.Client.Interfaces;
using NetSDK.Services.EngineEvaluator;
using NetSDK.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Client.Classes
{
    public class Client: IClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Client));
        private const string Control = "CONTROL";

        protected Splitter splitter;
        protected ISplitFetcher splitFetcher;
        protected Engine engine;

        public string GetTreatment(string key, string feature, Dictionary<string, object> attributes)
        {
            try
            {
                var split = splitFetcher.Fetch(feature);

                if (split == null)
                {
                    Log.Warn(String.Format("Unknown or invalid feature: {0}", feature));
                    return Control;
                }

                var treatment = engine.GetTreatment(key, split, attributes);

                return treatment;
            }
            catch(Exception e)
            {
                Log.Error(String.Format("Exception caught getting treatment for feature: {0}", feature), e);
                return Control;
            }
        }
    }
}
