using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DJ.Helper;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace DJ.Targets
{
    [Target(nameof(CacheTarget))]
    public class CacheTarget : Target
    {
        /// <summary>
        /// Look in the NLog.config if any target is already defined and returns it, otherwise a new one is registered
        /// </summary>
        /// <param name="defaultMaxCount">The maximum entries which should be buffered. Is only used if no target is defined</param>
        /// <param name="targetName">The name of the target you want to link with</param>
        /// <returns></returns>
        public static CacheTarget GetInstance(int defaultMaxCount = 0, string targetName = null)
        {
            if(LogManager.Configuration == null)
                LogManager.Configuration = new LoggingConfiguration();

            var predicate = PredicateBuilder.True<Target>().And(t => t is CacheTarget);
            if (!string.IsNullOrEmpty(targetName))
            {
                predicate = predicate.And(t => t.Name.Equals(targetName, StringComparison.CurrentCultureIgnoreCase) ||t.Name.Equals($"{targetName}_wrapped", StringComparison.CurrentCultureIgnoreCase));
            }
                
            var target = (CacheTarget)LogManager.Configuration.AllTargets.FirstOrDefault(predicate.Compile());
            if (target == null)
            {
                target = new CacheTarget { MaxCount = defaultMaxCount, Name = targetName ?? nameof(CacheTarget)};
                LogManager.Configuration.AddTarget(target.Name, target);
                LogManager.Configuration.LoggingRules.Insert(0, new LoggingRule("*", LogLevel.FromString("Trace"), target));
                LogManager.ReconfigExistingLoggers();
            }
            return target;
        }

        // ##############################################################################################################################
        // Properties
        // ##############################################################################################################################

        #region Properties

        // ##########################################################################################
        // Public Properties
        // ##########################################################################################

        /// <summary>
        /// The maximum amount of entries held in buffer/cache
        /// </summary>
        public int MaxCount { get; set; } = 100;
        
        public IObservable<LogEventInfo> Cache => _CacheSubject.AsObservable();
        private readonly ReplaySubject<LogEventInfo> _CacheSubject;

        // ##########################################################################################
        // Private Properties
        // ##########################################################################################

        #endregion

        // ##############################################################################################################################
        // Constructor
        // ##############################################################################################################################

        #region Constructor

        public CacheTarget()
        {
            _CacheSubject = new ReplaySubject<LogEventInfo>(MaxCount);
        }

        #endregion

        // ##############################################################################################################################
        // override
        // ##############################################################################################################################

        #region override

        protected override void Write(LogEventInfo logEvent)
        {
            _CacheSubject.OnNext(logEvent);
        }

        #endregion
    }
}