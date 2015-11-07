using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dashing.net.common;
using dashing.net.streaming;
using SODA;
//using SODA.Models;

namespace dashing.net.jobs
{
    [Export(typeof(IJob))]
    public abstract class OutageBase : IJob
    {
        protected Resource<Dictionary<string, object>> dataset;
        protected IEnumerable<Dictionary<string, object>> allRows;

        protected uint intervalSeconds = 10;

        public Lazy<Timer> Timer { get; set; }

        public OutageBase()
        {
            var domain = System.Web.Configuration.WebConfigurationManager.AppSettings["DataDomain"];
            var datasetId = System.Web.Configuration.WebConfigurationManager.AppSettings["DataSetId"];
            var appId = System.Web.Configuration.WebConfigurationManager.AppSettings["ApplicationId"];

            //initialize a new client
            //make sure you register for your own app token (http://dev.socrata.com/register)
            var client = new SodaClient(domain, appId);

            dataset = client.GetResource<Dictionary<string, object>>(datasetId);
            allRows = dataset.GetRows();

            var causes = allRows
                .GroupBy(r => r["cause"])
                .Select(g => new { Cause = g.Key, Count = g.Count() });

            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalSeconds)));
        }

        protected virtual void SendMessage(object message)
        {
        }
    }
}
