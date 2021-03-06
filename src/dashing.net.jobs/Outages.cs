﻿using System;
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
    public class Outages : OutageBase
    {
        public Outages()
        {
            intervalSeconds = 20;

            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalSeconds)));
        }

        protected new void SendMessage(object message)
        {
            try
            {
                allRows = dataset.GetRows();
                var total = allRows.Count();

                Dashing.SendMessage(new { value = total, id = "outages" });
            }
            catch (Exception)
            {
            }
        }
    }
}
