namespace WebApiInsight.Agent
{
    public interface IDbManager
    {
        void WriteMetrics(string measurement, object value);
    }
}
