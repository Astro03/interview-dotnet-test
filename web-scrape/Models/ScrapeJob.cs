using System;
using System.Collections.Generic;

namespace web_scrape.Models
{
    public class ScrapeJob
    {
        #region Private Member Variable
        /// <summary>
        /// Unique ID of the job
        /// </summary>
        private static int id = 1;
        #endregion Private Member Variable

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">url to scrape (i.e. http://www.google.com)</param>
        /// <param name="selector">css selector (i.e. li.active)</param>
        public ScrapeJob(string url, string selector)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url");
            }

            Id = id++;
            RequestedAt = DateTime.UtcNow;
            Url = url;
            Selector = selector;
            StatusEnum = ScrapeJobStatus.Pending;
            Result = new List<string>();
        }


        #region Public Properties
        /// <summary>
        /// Unique id of the job
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// DateTime Job was requested at 
        /// </summary>
        public DateTime RequestedAt { get; private set; }

        /// <summary>
        /// URL to scrape
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Selector to scrape (can be null)
        /// </summary>
        public string Selector { get; private set; }

        /// <summary>
        /// Status of the job
        /// </summary>
        public ScrapeJobStatus StatusEnum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status => StatusEnum.ToString();

        /// <summary>
        /// Result of the job
        /// </summary>
        public List<string> Result { get; set; }
        #endregion Public Properties
    }

    /// <summary>
    /// Possible States for the ScrapeJob
    /// </summary>
    public enum ScrapeJobStatus { Pending, Queued, InProgress, Completed, JobQueueError, NotFound, InvalidUrl, DcsoupError };
}