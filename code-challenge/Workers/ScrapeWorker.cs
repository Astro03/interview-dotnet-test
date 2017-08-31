﻿using code_challenge.Models;
using code_challenge.Services;
using Supremes;
using System;

namespace code_challenge.Workers
{
    /// <summary>
    /// workers who take jobs off the web scraper job queue.
    /// NOTE: we can technically put this worker on another box that just process the jobs as they come in
    /// </summary>
    public class ScrapeWorker // : IWorker  (where IWorker contains "void Run()")
    {
        #region Private Member Variable
        private JobQueue _JobQueue;
        private Cache _Cache;
        #endregion Private Member Variable

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ScrapeWorker()
        {
            _JobQueue = JobQueue.Instance;
            _Cache = Cache.Instance;
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Web page scrape worker
        /// Does the following in an infinite loop
        /// 1) takes a job from a blocking queue
        /// 2) change status in cache or data store
        /// 3) scrape the page
        /// 4) update job in cache or data store
        /// 5) repeat
        /// </summary>
        public void Run()
        {
            while (true)
            {
                var job = _JobQueue.Take();
                _Cache.UpdateJob(job.Id, ScrapeJobStatus.InProgress);
                _Scrape(job);
                _Cache.UpdateJob(job.Id, job.StatusEnum, job.Result); 
            }
        }
        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Scrape the page
        /// </summary>
        /// <param name="job"></param>
        private void _Scrape(ScrapeJob job)
        {
            try
            {
                var uri = new Uri(job.Url);
                var doc = Dcsoup.Parse(uri, 5000);
                if (!string.IsNullOrWhiteSpace(job.Selector))
                {
                    try
                    {
                        var tmp = doc.Select(job.Selector);
                        foreach (var t1 in tmp)
                        {
                            job.Result.Add(t1.Text);
                        }
                        job.StatusEnum = ScrapeJobStatus.Completed;
                    }
                    catch(Exception e)
                    {
                        job.StatusEnum = ScrapeJobStatus.DcsoupError;
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    var page = doc.ToString();
                    job.Result.Add(page);
                    job.StatusEnum = ScrapeJobStatus.Completed;
                }
            }
            catch(UriFormatException ufe)
            {
                job.StatusEnum = ScrapeJobStatus.InvalidUrl;
                Console.WriteLine(ufe);
            }
            catch (Exception e)
            {
                job.StatusEnum = ScrapeJobStatus.DcsoupError;
                Console.WriteLine(e);
            }
        }
        #endregion Private Methods
    }
}