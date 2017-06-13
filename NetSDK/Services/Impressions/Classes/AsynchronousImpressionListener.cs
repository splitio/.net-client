using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splitio.Services.Impressions.Interfaces;
using Splitio.Domain;
using System.Threading.Tasks;
using Common.Logging;
using System.Diagnostics;


namespace Splitio.Services.Impressions.Classes
{
    public class AsynchronousImpressionListener : IImpressionListener
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AsynchronousImpressionListener));
        private List<IImpressionListener> workers = new List<IImpressionListener>();

        public void AddListener(IImpressionListener worker)
        {
            workers.Add(worker);
        }

        public void Log(KeyImpression impression)
        {
            try
            {
                //This task avoids waiting to fire and forget 
                //all worker's tasks in the main thread
                var listenerTask =
                    new Task(() =>
                            {
                                foreach (IImpressionListener worker in workers)
                                {
                                    try
                                    {
                                        //This task makes worker.Log() run independently 
                                        //and avoid one worker to block another.
                                        var logTask =
                                            new Task(() =>
                                            {
                                                var stopwatch = Stopwatch.StartNew();
                                                worker.Log(impression);
                                                stopwatch.Stop();
                                                Logger.Info(worker.GetType() + " took " + stopwatch.ElapsedMilliseconds + " milliseconds");
                                            });
                                        logTask.Start();
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("Exception performing Log with worker. ", e);
                                    }
                                }
                            });
                listenerTask.Start();
            }
            catch (Exception e)
            {
                Logger.Error("Exception creating Log task. ", e);
            }
        }
    }
}
