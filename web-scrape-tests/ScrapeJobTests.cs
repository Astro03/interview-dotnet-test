using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using web_scrape.Models;

namespace web_scrape_tests
{
    [TestClass]
    public class ScrapeJobTests
    {
        /// <summary>
        /// Test ScrapeJob unique ID is working (auto-incrementing)
        /// </summary>
        [TestMethod]
        public void IncrementIdTest()
        {
            var scrape1 = new ScrapeJob("url1", null);
            var scrape2 = new ScrapeJob("url2", null);

            Assert.AreNotEqual(scrape1.Id, scrape2.Id);
        }

        /// <summary>
        /// Testing Constructor (i.e. url not null or null)
        /// </summary>
        [TestMethod]
        public void ConstrutorTest()
        {
            var scrape1 = new ScrapeJob("url1", null);

            Assert.AreNotEqual(null, scrape1.Url);

            Exception exception = null;
            try
            {
                var scrape2 = new ScrapeJob(null, null);
            }
            catch (ArgumentNullException ex)
            {
                exception = ex;
            }

            Assert.AreNotEqual(null, exception);
        }


    }
}
