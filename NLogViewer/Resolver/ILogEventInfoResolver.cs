using NLog;

namespace DJ.Resolver
{
    /// <summary>
    /// Default interface to resolve a <see cref="LogEventInfo"/> for your personal needs
    /// </summary>
    public interface ILogEventInfoResolver
    {
        string Resolve(LogEventInfo logEventInfo);
    }
}
