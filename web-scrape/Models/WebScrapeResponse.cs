namespace web_scrape.Models
{
    /// <summary>
    /// Standard response to API call
    /// </summary>
    public class WebScrapeResponse
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="job">scrape job</param>
        /// <param name="errorCode">Result of API call</param>
        public WebScrapeResponse(ScrapeJob job, ErrorCode errorCode)
        {
            ScrapeJob = job;
            ErrorCode = errorCode;
        }
        #endregion Constructor

        #region Public Properties
        /// <summary>
        /// Scrape Job 
        /// </summary>
        public ScrapeJob ScrapeJob { get; private set; }

        /// <summary>
        /// Error Code Enum
        /// </summary>
        public ErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// Error Code String
        /// </summary>
        public string ErrorCodeString { get { return ErrorCode.ToString(); } }
        #endregion Public Properties
    }

    /// <summary>
    /// Possible Error Codes for API Web Scrape Responses
    /// </summary>
    public enum ErrorCode { InvalidInput, Success, NotFound, FailedQueue };
}
