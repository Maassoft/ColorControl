using NLog;

namespace DJ.Resolver
{
    public class TimeStampResolver : ILogEventInfoResolver
    {
        public string Resolve(LogEventInfo logEventInfo)
        {
            return logEventInfo.TimeStamp.ToString("dd-MM-yyyy HH:mm:ss.fff");
        }
    }
}