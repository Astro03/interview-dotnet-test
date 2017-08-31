using code_challenge.Models;
using System.Collections.Generic;

namespace code_challenge.Services
{
    /// <summary>
    /// Can substitute with a memcache  - have all objects come from a same ICache interface
    /// todo: we should make it have an interface of some short that aligns with it how typical caches work
    /// </summary>
    public class Cache
    {
        #region Private Member Variable
        private static Dictionary<long, ScrapeJob> _Jobs = new Dictionary<long, ScrapeJob>();
        #endregion Private Member Variable


        #region Singleton Pattern
        private static volatile Cache _Instance;
        private static object syncRoot = new object();

        private Cache() { }

        public static Cache Instance
        {
            get
            {
                if (null == _Instance)
                {
                    lock (syncRoot)
                    {
                        if (null == _Instance)
                        {
                            _Instance = new Cache();
                        }
                    }
                }
                return _Instance;
            }
        }
        #endregion Singleton Pattern

        #region Public Methods
        /// <summary>
        /// Get job if in Cache
        /// </summary>
        /// <param name="id">id of job</param>
        /// <returns>ScrapeJob if found, null otherwise</returns>
        public ScrapeJob GetJob(long id)
        {
            var retVal = (ScrapeJob)null;
            if (_Jobs.ContainsKey(id))
            {
                retVal = _Jobs[id];
            }
            return retVal;
        }

        /// <summary>
        /// Add job to cache
        /// </summary>
        /// <param name="job">job to add to cache</param>
        /// <returns>true if added, false otherwise</returns>
        public bool AddJob(ScrapeJob job)
        {
            if (null == job)
            {
                return false;
            }

            lock (syncRoot)
            {
                if (_Jobs.ContainsKey(job.Id))
                {
                    return false;
                }
                else
                {
                    _Jobs.Add(job.Id, job);
                }
            }
            return true;
        }

        /// <summary>
        /// Update Job in cache
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool UpdateJob(long id, ScrapeJobStatus status, List<string> result=null)
        {
            var retVal = false;
            lock (syncRoot)
            {
                if (_Jobs.ContainsKey(id))
                {
                    _Jobs[id].StatusEnum = status;
                    if (null != result)
                    {
                        _Jobs[id].Result = result;
                    }
                    retVal = true;
                }
            }
            return retVal;
        }
        #endregion Public Methods

    }
}