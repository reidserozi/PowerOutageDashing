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
    public class Causes : OutageBase
    {
        public Causes()
        {
            intervalSeconds = 15;

            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalSeconds)));
        }

        protected new void SendMessage(object message)
        {
            allRows = dataset.GetRows();
            var causes = allRows
               .GroupBy(r => r["cause"])
               .Select(g => new { Cause = g.Key, Count = g.Count() });

            Dashing.SendMessage(new { id = "causes", items = causes.Select(m => new { label = m.Cause, value = m.Count }) });
        }
    }
}
