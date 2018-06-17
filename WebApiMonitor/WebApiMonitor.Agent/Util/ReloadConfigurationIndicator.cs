namespace WebApiMonitor.Agent
{
    public class ReloadConfigurationIndicator
    {
        private bool _aspCollectorFlag = false;
        private bool _processCollectorFlag = false;
        private object _locker = new object();

        public void SetFlag ()
        {
            lock(_locker)
            {
                _aspCollectorFlag = true;
                _processCollectorFlag = true;
            }
        }

        public bool GetAndResetAspCollectorFlag ()
        {
            lock (_locker)
            {
                var result = _aspCollectorFlag;
                _aspCollectorFlag = false;
                return result;
            }
        }

        public bool GetAndResetProcessCollectorFlag ()
        {
            lock (_locker)
            {
                var result = _processCollectorFlag;
                _processCollectorFlag = false;
                return result;
            }
        }
    }
}