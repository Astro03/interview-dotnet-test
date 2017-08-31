using code_challenge.Models;
using code_challenge.Services;
using Microsoft.AspNetCore.Mvc;

namespace code_challenge.Controllers
{
    /// <summary>
    /// Checks status of job (get), submitting a job (post)
    /// </summary>
    [Route("api/[controller]")]
    public class ScrapeController : Controller
    {
        #region Private Member Variables
        /// <summary>
        /// Cache 
        /// </summary>
        private Cache _Cache => Cache.Instance;

        /// <summary>
        /// Job Queue
        /// </summary>
        private JobQueue _JobQueue => JobQueue.Instance;
        #endregion Private Member Variables

        #region Public Methods
        /// <summary>
        /// GET api/values/5
        /// </summary>
        /// <param name="id">id of job we want to find</param>
        /// <returns>AstroResponse with Error Code</returns>
        [HttpGet("{id}")]
        public WebScrapeResponse GetJob(int id)
        {
            var job = _Cache.GetJob(id);
            var resp = new WebScrapeResponse(job, null == job ? ErrorCode.NotFound : ErrorCode.Success);
            return resp;
        }

        /// <summary>
        /// POST api/scrape
        /// </summary>
        /// <param name="req"></param>
        /// <returns>AstroResponse with Error Code</returns>
        [HttpPost]
        public WebScrapeResponse CreateJob([FromBody]WebScrapeRequest req)
        {
            // basic error checking
            if (null == req || string.IsNullOrWhiteSpace(req.Url))
            {
                return new WebScrapeResponse(null, ErrorCode.InvalidInput);
            }

            // create job
            var job = new ScrapeJob(req.Url, req.Selector);

            // send job to message queue and data store (cache) 
            // NOTE: this can cause issues if we don't sync properly
            var worked = _JobQueue.Add(job) && _Cache.AddJob(job);

            var resp = new WebScrapeResponse(job, worked ? ErrorCode.Success : ErrorCode.FailedQueue);
            return resp;
        }
        #endregion Public Methods
    }
}
