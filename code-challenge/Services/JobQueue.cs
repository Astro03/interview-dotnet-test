using code_challenge.Models;
using System;
using System.Collections.Concurrent;

namespace code_challenge.Services
{
    /// <summary>
    /// ideally something like rabbitMQ or AmazonSQS
    /// NOTE: for this project, we will spoof something like something. we will use a static blocking queue instead 
    /// </summary>
    public class JobQueue
    {

        #region Private Member Variable
        /// <summary>
        /// Job queue
        /// </summary>
        private BlockingCollection<ScrapeJob> Jobs = new BlockingCollection<ScrapeJob>();
        #endregion Private Member Variable

        #region Singleton Pattern
        private static volatile JobQueue _Instance;
        private static object syncRoot = new object();

        private JobQueue() { }

        public static JobQueue Instance
        {
            get
            {
                if (null == _Instance)
                {
                    lock (syncRoot)
                    {
                        if (null == _Instance)
                        {
                            _Instance = new JobQueue();
                        }
                    }
                }
                return _Instance;
            }
        }
        #endregion Singleton Pattern

        #region Public Methods
        /// <summary>
        /// Add Item to the Job Queue
        /// </summary>
        /// <param name="job">job to add</param>
        /// <returns>true if add was successful, false otherwise</returns>
        public bool Add(ScrapeJob job)
        {
            var retVal = true;
            try
            {
                job.StatusEnum = ScrapeJobStatus.Queued;
                Jobs.Add(job);
            }
            catch (Exception e)
            {
                // put logging here
                Console.WriteLine(e);
                job.StatusEnum = ScrapeJobStatus.JobQueueError;
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Take an item from the queue, if one is not there, wait here until one exists
        /// </summary>
        /// <returns>ScrapeJob</returns>
        public ScrapeJob Take()
        {
            return Jobs.Take();
        }
        #endregion Public Methods
    }
}
