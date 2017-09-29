using web_scrape.Models;
using web_scrape.Services;
using Microsoft.AspNetCore.Mvc;

namespace web_scrape.Controllers
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
        /// <returns>result of getting a job</returns>
        [HttpGet("{id}", Name = "GetJob")]
        public IActionResult GetJob(int id)
        {
            var job = _Cache.GetJob(id);
            return (job == null) ?
                new NotFoundObjectResult(new { err_msg = "Job with id not found", id = id }) :
                (IActionResult)new OkObjectResult(job.GetJson(true));
        }

        /// <summary>
        /// POST api/scrape
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Result of Job creation</returns>
        [HttpPost]
        public IActionResult CreateJob([FromBody]WebScrapeRequest req)
        {
            // basic error checking
            if (null == req || string.IsNullOrWhiteSpace(req.Url))
            {
                return new BadRequestObjectResult(new { err_msg = "Invalid input" });
            }

            // create job
            var job = new ScrapeJob(req.Url, req.Selector);

            // send job to message queue and data store (cache) 
            // NOTE: this can cause issues if we don't sync properly
            var worked = _JobQueue.Add(job) && _Cache.AddJob(job);

            var json = job.GetJson();
            return (worked) ?
                new CreatedAtRouteResult("GetJob", new { id = job.Id }, json) :
                new ObjectResult(json) { StatusCode = 500 };
        }
        #endregion Public Methods
    }
}
