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
        static ReloadConfigurationIndicator reloadConfigIndicator = new ReloadConfigurationIndicator();
        static readonly ILog _logger = LogHelper.GetLogger();

        static void Main()
        {
            pauseOrStartEvent.Set();

            var influxDbManager = new InfluxDbManager(_logger);
            var processCollector = new ProcessCollector(_logger, influxDbManager, pauseOrStartEvent, reloadConfigIndicator);
            var aspNetCollector = new AspNetCollector(_logger, influxDbManager, pauseOrStartEvent, reloadConfigIndicator);
            var iisCollector = new IisLogCollector(_logger, influxDbManager, 
                ProcessHelper.GetLogsPath(Settings.AppName, Settings.PoolName));
            var collectorThreads = new List<Thread>
            {
                new Thread(processCollector.Start),
                new Thread(aspNetCollector.Start),
                new Thread(iisCollector.Start),
            };
            collectorThreads.ForEach(t => t.Start());
            StartRestServer();
        }

        static void StartRestServer()
        {
            var config = new HttpSelfHostConfiguration("http://localhost:" + Settings.ServerPort);
            config.MessageHandlers.Add(new EventsDelegatingHandler(pauseOrStartEvent, reloadConfigIndicator));
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

    class EventsDelegatingHandler : DelegatingHandler
    {
        private ManualResetEvent _pauseOrStartEvent;
        private ReloadConfigurationIndicator _reloadConfigIndicator;

        public EventsDelegatingHandler(ManualResetEvent pauseOrStartEvent, ReloadConfigurationIndicator reloadConfigIndicator)
        {
            _pauseOrStartEvent = pauseOrStartEvent;
            _reloadConfigIndicator = reloadConfigIndicator;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Properties["StartStopEvent"] = _pauseOrStartEvent;
            request.Properties["ReloadConfigurationIndicator"] = _reloadConfigIndicator;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
