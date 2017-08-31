using web_scrape.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;

namespace web_scrape
{
    public class Startup
    {
        #region Worker Threads
        /// <summary>
        /// Max Number of threads for the Scrape Worker
        /// </summary>
        private const int _maxThreads = 3;

        /// <summary>
        /// Worker Threads
        /// </summary>
        private static List<Thread> _WorkerThreads = new List<Thread>();
        #endregion Worker Threads

        public Startup(IHostingEnvironment env)
        {
            // start worker threads (background) 
            // these could be on own separate worker boxes
            for (int i = 0; i < _maxThreads; i++)
            {
                var worker = new ScrapeWorker();
                var thread = new Thread(new ThreadStart(worker.Run))
                {
                    Name = $"alpha_{i}"
                };
                thread.Start();
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
