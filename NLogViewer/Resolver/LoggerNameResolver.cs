using NLog;

namespace DJ.Resolver
{
    public class LoggerNameResolver : ILogEventInfoResolver
    {
        public string Resolve(LogEventInfo logEventInfo)
        {
            return logEventInfo.LoggerName;
        }
    }
}