using System.Diagnostics;

namespace WebApiMonitor.Agent
{
    public class MeasurementData
    {
        public string Measurement { get; set; }
        public PerformanceCounter Counter { get; set; }
    }
}
