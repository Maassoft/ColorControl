using NLog;

namespace DJ.Resolver
{
    public class IdResolver : ILogEventInfoResolver
    {
        public string Resolve(LogEventInfo logEventInfo)
        {
            return logEventInfo.SequenceID.ToString();
        }
    }
}