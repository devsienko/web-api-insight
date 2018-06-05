using System;
using WebApiMonitor.Agent.Util;
using log4net;
using System.Threading;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Threading.Tasks;

namespace WebApiMonitor.Agent
{
    class Program
    {
        static ManualResetEvent pauseOrStartEvent = new ManualResetEvent(false);
        static readonly ILog _logger = LogHelper.GetLogger();

        static void Main()
        {
            pauseOrStartEvent.Set();

            var influxDbManager = new InfluxDbManager(_logger);
            var processCollector = new ProcessCollector(_logger, influxDbManager, pauseOrStartEvent);
            var aspNetCollector = new AspNetCollector(_logger, influxDbManager, pauseOrStartEvent);
            var collectorThreads = new List<Thread>
            {
                new Thread(processCollector.Start),
                new Thread(aspNetCollector.Start),
            };
            //new Thread(DoSomething).Start();

            collectorThreads.ForEach(t => t.Start());

            StartRestServer();

            var iisReader = new IisLogCollector(_logger, influxDbManager, ProcessHelper.GetLogsPath(Settings.AppName, Settings.PoolName));
            iisReader.Process();
        }

        //static void DoSomething()
        //{
        //    while(true)
        //    {
        //        var t = false;
        //        if (t)
        //        {
        //            pauseOrStartEvent.Reset();
        //            pauseOrStartEvent.Set();
        //        }
        //        Thread.Sleep(1000);
        //    }
        //}

        static void StartRestServer()
        {
            var config = new HttpSelfHostConfiguration("http://localhost:" + Settings.ServerPort);

            config.MessageHandlers.Add(new Test(pauseOrStartEvent));

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional });

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }
    }

    class Test : DelegatingHandler
    {
        private ManualResetEvent _pauseOrStartEvent;
        public Test(ManualResetEvent pauseOrStartEvent)
        {
            _pauseOrStartEvent = pauseOrStartEvent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Properties["StartStopEvent"] = _pauseOrStartEvent;
            return base.SendAsync(request, cancellationToken);
            //throw new NotImplementedException();
        }
    }
}
