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
        private List<IImpressionListener> workers = new List<IImpressionListener>();

        public void AddListener(IImpressionListener worker)
        {
            workers.Add(worker);
        }

        public void Log(KeyImpression impression)
        {
            try
            {
                var enqueueTask = new Task(() => 
                    {
                        foreach (IImpressionListener worker in workers)
                        {
                            try
                            {
                                worker.Log(impression);
                            }
                            catch (Exception e)
                            {
                                Logger.Error("Exception performing Log with worker. ", e);
                            }
                        }
                    });
                    
                enqueueTask.Start();
            }
            catch (Exception e)
            {
                Logger.Error("Exception creating Log task. ", e);
            }
        }
    }
}
