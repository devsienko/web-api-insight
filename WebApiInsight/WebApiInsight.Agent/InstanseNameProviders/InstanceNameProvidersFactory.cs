using System;

namespace WebApiInsight.Agent
{
    public class InstanceNameProvidersFactory
    {
        public IInstanceNameProvider GetNameProvider(MetricSource ms)
        {
            IInstanceNameProvider result;
            if (ms == MetricSource.IisNode)
                result = new IisNodeInstanceNameProvider();
            else if (ms == MetricSource.W3Wp)
                result = new W3WpInstanceNameProvider();
            else
                throw new InvalidOperationException();
            return result;
        }
    }

    public enum MetricSource
    {
        IisNode,
        W3Wp
    }
}
