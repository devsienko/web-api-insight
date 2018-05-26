using Vibrant.InfluxDB.Client.Rows;

namespace WebApiInsight.Agent
{
    public interface IDbManager
    {
        void WriteMetrics(string measurement, DynamicInfluxRow props);
        void WriteMetricsValue(string measurement, object value);
    }
}
