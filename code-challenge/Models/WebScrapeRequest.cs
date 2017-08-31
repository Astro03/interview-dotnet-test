namespace code_challenge.Models
{
    /// <summary>
    /// Web Scrape Post Request
    /// </summary>
    public class WebScrapeRequest
    {
        /// <summary>
        /// Url to scrape
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Selector for Web Scraping (can be null)
        /// </summary>
        public string Selector { get; set; }
    }
}
