using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splitio.Services.Impressions.Interfaces;
using Splitio.Domain;
using System.Threading.Tasks;
using log4net;


namespace Splitio.Services.Impressions.Classes
{
    public class AsynchronousImpressionListener : IImpressionListener
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(AsynchronousImpressionListener));
        private IImpressionListener worker;

        public AsynchronousImpressionListener(IImpressionListener worker)
        {
            this.worker = worker;
        }

        public void Log(KeyImpression impression)
        {
            try
            {
                var enqueueTask = new Task(() => worker.Log(impression));
                enqueueTask.Start();
            }
            catch (Exception e)
            {
                Logger.Error("Exception performing Log. ", e);
            }
        }
    }
}
