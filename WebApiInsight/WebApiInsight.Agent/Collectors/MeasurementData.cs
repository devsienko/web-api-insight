using System.Diagnostics;

namespace WebApiInsight.Agent
{
    public class MeasurementData
    {
        public string Measurement { get; set; }
        public PerformanceCounter Counter { get; set; }
    }
}
