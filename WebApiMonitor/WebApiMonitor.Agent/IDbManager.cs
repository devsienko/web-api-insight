using Vibrant.InfluxDB.Client.Rows;

namespace WebApiMonitor.Agent
{
    public interface IDbManager
    {
        void WriteMetrics(string measurement, DynamicInfluxRow props);
        void WriteMetricsValue(string measurement, object value);
    }
}
