***Astro is Awesome***
***Author : Astro Ashtaralnakhai***

**Foreword**
* This was the first time I've built an API with .NET. I followed instructions on how to create a web API from https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-vsc.
* This application was designed in this manner in order to do something different than what was already present in this fork https://github.com/samrahimi/interview-dotnet-test. I would like to suggest that you remove the fork or else others can copy it.
* Dcsoup Nuget Package is used for the Web Scraping.

**High Level Design**
* Requests come in to the server
* Controller then sends message to "JobQueue" class (can be RabbitMQ or AmazonSQS) AND the "Cache" (can be memcache or Amazon ElastiCache)
* "ScrapeWorker" reads messages from "JobQueue", updates "Cache", scrapes the web page, and then updates "Cache" with result
* Since we are not using a DB, we are using the "Cache" as the main data store
* "Startup" launches workers for this application
    * For production, we should have new boxes (and a separate application) to run the workers

**Usage**
* To request, POST /api/scrape with url and selector in the body as a json
    * i.e. { "url": "http://www.mangastream.com", "selector":"li.active" }
		* where url is a full url
        * where selector is a css selector
    * On a successful POST the response will contain
		* errorCodeString="Success" 
		* "scrapeJob" with numeric "id" - also known as the job id
    * On an unsuccessful POST the response will contain
		* errorCodeString="XXX" where XXX is an error
* To get status or view results, GET /api/scrape/{id}
    * "id" is will be found in the successful POST response

**Improvements**
* Do error checking in method calls
* Validate all inputs in controller
    * i.e. url, selector, id
* More granular error messages in AstroResponse
* Logging
* Instrumentation of application
* Setting File
    * Read in number of worker threads from settings file
	* Add other items into settings file (such as if we want to use RabbitMQ instead of JobQueue class)
* JobQueue 
    * Have an Interface which is common to all Queuing services 
    * Swap class with RabbitMQ or AmazonSQS
* Cache would have an Interface which is common to all caching services
    * Have an Interface which is common to all caching services 
    * Swap class with memcache or Amazon ElastiCache
* Workers
    * Workers will be on a separate machine
    * Web Scraper will be in-house made instead of 3rd party vendor

**List of Classes**
* Program
	* Generated class to start the web server
* Startup
	* Generated class that runs on startup
	* Launches ScapeWorker Background threads  
* ScrapeController
	* API Controller for web scraping
* WebScrapeRequest
	* POST web scrape variables
* WebScrapeResponse
	* Response for calls to ScrapeController
* ScrapeJob
	* Contains status of job, results, and everything else related to the web scrape job
* Cache
	* Cache used by application as datastore
* JobQueue
	* Blocking Queue used by application to send messages to Workers
* ScrapeWorker
	* Waits in an infinite loop for items to be in the job queue, then processes the job