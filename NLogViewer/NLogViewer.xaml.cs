using DJ.Extensions;
using DJ.Resolver;
using DJ.Targets;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DJ
{
    /// <summary>
    /// Interaktionslogik für NLogViewer.xaml
    /// </summary>
    public partial class NLogViewer : UserControl
    {
        // ##############################################################################################################################
        // Dependency Properties
        // ##############################################################################################################################

        #region Dependency Properties

        // ##########################################################################################
        // Colors
        // ##########################################################################################

        #region Colors

        /// <summary>
        /// The background for the trace output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush TraceBackground
        {
            get => (Brush)GetValue(TraceBackgroundProperty);
            set => SetValue(TraceBackgroundProperty, value);
        }

        /// <summary>
        /// The <see cref="TraceBackground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty TraceBackgroundProperty =
            DependencyProperty.Register("TraceBackground", typeof(Brush), typeof(NLogViewer),
                new PropertyMetadata((Brush)(new BrushConverter().ConvertFrom("#D3D3D3"))));

        /// <summary>
        /// The foreground for the trace output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush TraceForeground
        {
            get => (Brush)GetValue(TraceForegroundProperty);
            set => SetValue(TraceForegroundProperty, value);
        }

        /// <summary>
        /// The <see cref="TraceForeground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty TraceForegroundProperty =
            DependencyProperty.Register("TraceForeground", typeof(Brush), typeof(NLogViewer),
                new PropertyMetadata((Brush)(new BrushConverter().ConvertFrom("#042271"))));

        /// <summary>
        /// The background for the debug output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush DebugBackground
        {
            get => (Brush)GetValue(DebugBackgroundProperty);
            set => SetValue(DebugBackgroundProperty, value);
        }

        /// <summary>
        /// The <see cref="DebugBackground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty DebugBackgroundProperty =
            DependencyProperty.Register("DebugBackground", typeof(Brush), typeof(NLogViewer),
                new PropertyMetadata((Brush)(new BrushConverter().ConvertFrom("#90EE90"))));

        /// <summary>
        /// The foreground for the debug output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush DebugForeground
        {
            get => (Brush)GetValue(DebugForegroundProperty);
            set => SetValue(DebugForegroundProperty, value);
        }

        /// <summary>
        /// The <see cref="DebugForeground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty DebugForegroundProperty =
            DependencyProperty.Register("DebugForeground", typeof(Brush), typeof(NLogViewer),
                new PropertyMetadata((Brush)(new BrushConverter().ConvertFrom("#042271"))));

        /// <summary>
        /// The background for the info output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush InfoBackground
        {
            get => (Brush)GetValue(InfoBackgroundProperty);
            set => SetValue(InfoBackgroundProperty, value);
        }

        /// <summary>
        /// The <see cref="InfoBackground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty InfoBackgroundProperty = DependencyProperty.Register("InfoBackground",
            typeof(Brush), typeof(NLogViewer),
            new PropertyMetadata((Brush)(new BrushConverter().ConvertFrom("#0000FF"))));

        /// <summary>
        /// The foreground for the info output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush InfoForeground
        {
            get => (Brush)GetValue(InfoForegroundProperty);
            set => SetValue(InfoForegroundProperty, value);
        }

        /// <summary>
        /// The <see cref="InfoForeground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty InfoForegroundProperty = DependencyProperty.Register("InfoForeground",
            typeof(Brush), typeof(NLogViewer), new PropertyMetadata(Brushes.White));

        /// <summary>
        /// The background for the warn output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush WarnBackground
        {
            get => (Brush)GetValue(WarnBackgroundProperty);
            set => SetValue(WarnBackgroundProperty, value);
        }

        /// <summary>
        /// The <see cref="WarnBackground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty WarnBackgroundProperty = DependencyProperty.Register("WarnBackground",
            typeof(Brush), typeof(NLogViewer),
            new PropertyMetadata((Brush)(new BrushConverter().ConvertFrom("#FFFF00"))));

        /// <summary>
        /// The foreground for the warn output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush WarnForeground
        {
            get => (Brush)GetValue(WarnForegroundProperty);
            set => SetValue(WarnForegroundProperty, value);
        }

        /// <summary>
        /// The <see cref="WarnForeground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty WarnForegroundProperty = DependencyProperty.Register("WarnForeground",
            typeof(Brush), typeof(NLogViewer),
            new PropertyMetadata((Brush)(new BrushConverter().ConvertFrom("#324B5C"))));

        /// <summary>
        /// The background for the error output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush ErrorBackground
        {
            get => (Brush)GetValue(ErrorBackgroundProperty);
            set => SetValue(ErrorBackgroundProperty, value);
        }

        /// <summary>
        /// The <see cref="ErrorBackground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ErrorBackgroundProperty =
            DependencyProperty.Register("ErrorBackground", typeof(Brush), typeof(NLogViewer),
                new PropertyMetadata(Brushes.Red));

        /// <summary>
        /// The foreground for the error output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush ErrorForeground
        {
            get => (Brush)GetValue(ErrorForegroundProperty);
            set => SetValue(ErrorForegroundProperty, value);
        }

        /// <summary>
        /// The <see cref="ErrorForeground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ErrorForegroundProperty =
            DependencyProperty.Register("ErrorForeground", typeof(Brush), typeof(NLogViewer),
                new PropertyMetadata(Brushes.White));

        /// <summary>
        /// The background for the fatal output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush FatalBackground
        {
            get => (Brush)GetValue(FatalBackgroundProperty);
            set => SetValue(FatalBackgroundProperty, value);
        }

        /// <summary>
        /// The <see cref="FatalBackground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty FatalBackgroundProperty =
            DependencyProperty.Register("FatalBackground", typeof(Brush), typeof(NLogViewer),
                new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// The foreground for the fatal output
        /// </summary>
        [Category("NLogViewerColors")]
        public Brush FatalForeground
        {
            get => (Brush)GetValue(FatalForegroundProperty);
            set => SetValue(FatalForegroundProperty, value);
        }

        /// <summary>
        /// The <see cref="FatalForeground"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty FatalForegroundProperty =
            DependencyProperty.Register("FatalForeground", typeof(Brush), typeof(NLogViewer),
                new PropertyMetadata(Brushes.Yellow));

        #endregion

        // ##########################################################################################
        // NLogViewer
        // ##########################################################################################

        #region NLogViewer

        /// <summary>
        /// Is looking if any target with this name is configured and tries to link it
        /// </summary>
        [Category("NLogViewer")]
        public string TargetName
        {
            get => (string)GetValue(TargetNameProperty);
            set => SetValue(TargetNameProperty, value);
        }

        /// <summary>
        /// The <see cref="TargetName"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty TargetNameProperty = DependencyProperty.Register("TargetName", typeof(string), typeof(NLogViewer), new PropertyMetadata(null));

        /// <summary>
        /// Private DP to bind to the gui
        /// </summary>
        [Category("NLogViewer")]
        public CollectionViewSource LogEvents
        {
            get => (CollectionViewSource)GetValue(LogEventsProperty);
            private set => SetValue(LogEventsProperty, value);
        }

        /// <summary>
        /// The <see cref="LogEvents"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty LogEventsProperty = DependencyProperty.Register("LogEvents",
            typeof(CollectionViewSource), typeof(NLogViewer), new PropertyMetadata(null));

        /// <summary>
        /// Automatically scroll to the newest entry
        /// </summary>
        [Category("NLogViewer")]
        public bool AutoScroll
        {
            get => (bool)GetValue(AutoScrollProperty);
            set => SetValue(AutoScrollProperty, value);
        }

        /// <summary>
        /// The <see cref="AutoScroll"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty AutoScrollProperty = DependencyProperty.Register("AutoScroll", typeof(bool), typeof(NLogViewer), new PropertyMetadata(true, AutoScrollChangedCallback));

        private static void AutoScrollChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is NLogViewer instance)
            {
                instance.OnAutoScrollChanged();
            }
        }

        protected virtual void OnAutoScrollChanged()
        {
            if (AutoScroll)
                ListView?.ScrollToEnd();
        }

        /// <summary>
        /// Delele all entries
        /// </summary>
        [Category("NLogViewer")]
        public ICommand ClearCommand
        {
            get => (ICommand)GetValue(ClearCommandProperty);
            set => SetValue(ClearCommandProperty, value);
        }

        /// <summary>
        /// The <see cref="ClearCommand"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ClearCommandProperty = DependencyProperty.Register("ClearCommand",
            typeof(ICommand), typeof(NLogViewer), new PropertyMetadata(null));

        /// <summary>
        /// Stop logging
        /// </summary>
        [Category("NLogViewer")]
        public bool Pause
        {
            get => (bool)GetValue(PauseProperty);
            set => SetValue(PauseProperty, value);
        }

        /// <summary>
        /// The <see cref="Pause"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty PauseProperty = DependencyProperty.Register("Pause", typeof(bool), typeof(NLogViewer), new PropertyMetadata(false));

        /// <summary>
        /// The maximum number of entries before automatic cleaning is performed. There is a hysteresis of 100 entries which must be exceeded.
        /// Example: <see cref="MaxCount"/> is '1000'. Then after '1100' entries, everything until '1000' is deleted.
        /// If set to '0' or less, it is deactivated
        /// </summary>
        [Category("NLogViewer")]
        public int MaxCount
        {
            get => (int)GetValue(MaxCountProperty);
            set => SetValue(MaxCountProperty, value);
        }

        /// <summary>
        /// The <see cref="MaxCount"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty MaxCountProperty = DependencyProperty.Register("MaxCount", typeof(int), typeof(NLogViewer), new PropertyMetadata(5000));


        #endregion

        // ##########################################################################################
        // Resolver
        // ##########################################################################################

        #region Resolver

        /// <summary>
        /// The <see cref="ILogEventInfoResolver"/> to format the id
        /// </summary>
        [Category("NLogViewerResolver")]
        public ILogEventInfoResolver IdResolver
        {
            get => (ILogEventInfoResolver)GetValue(IdResolverProperty);
            set => SetValue(IdResolverProperty, value);
        }

        /// <summary>
        /// The <see cref="IdResolver"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IdResolverProperty = DependencyProperty.Register("IdResolver", typeof(ILogEventInfoResolver), typeof(NLogViewer), new PropertyMetadata(new IdResolver()));

        /// <summary>
        /// The <see cref="ILogEventInfoResolver"/> to format the timestamp output
        /// </summary>
        [Category("NLogViewerResolver")]
        public ILogEventInfoResolver TimeStampResolver
        {
            get => (ILogEventInfoResolver)GetValue(TimeStampResolverProperty);
            set => SetValue(TimeStampResolverProperty, value);
        }

        /// <summary>
        /// The <see cref="TimeStampResolver"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty TimeStampResolverProperty = DependencyProperty.Register("TimeStampResolver", typeof(ILogEventInfoResolver), typeof(NLogViewer), new PropertyMetadata(new TimeStampResolver()));

        /// <summary>
        /// The <see cref="ILogEventInfoResolver"/> to format the loggername
        /// </summary>
        [Category("NLogViewerResolver")]
        public ILogEventInfoResolver LoggerNameResolver
        {
            get => (ILogEventInfoResolver)GetValue(LoggerNameResolverProperty);
            set => SetValue(LoggerNameResolverProperty, value);
        }

        /// <summary>
        /// The <see cref="LoggerNameResolver"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty LoggerNameResolverProperty = DependencyProperty.Register("LoggerNameResolver", typeof(ILogEventInfoResolver), typeof(NLogViewer), new PropertyMetadata(new LoggerNameResolver()));

        /// <summary>
        /// The <see cref="ILogEventInfoResolver"/> to format the message
        /// </summary>
        [Category("NLogViewerResolver")]
        public ILogEventInfoResolver MessageResolver
        {
            get => (ILogEventInfoResolver)GetValue(MessageResolverProperty);
            set => SetValue(MessageResolverProperty, value);
        }

        /// <summary>
        /// The <see cref="MessageResolver"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty MessageResolverProperty = DependencyProperty.Register("MessageResolver", typeof(ILogEventInfoResolver), typeof(NLogViewer), new PropertyMetadata(new MessageResolver()));

        #endregion


        #endregion

        // ##############################################################################################################################
        // Properties
        // ##############################################################################################################################

        #region Properties

        // ##########################################################################################
        // Public Properties
        // ##########################################################################################



        // ##########################################################################################
        // Private Properties
        // ##########################################################################################

        private ObservableCollection<LogEventInfo> _LogEventInfos { get; } = new ObservableCollection<LogEventInfo>();
        private IDisposable _Subscription;
        private Window _ParentWindow;

        #endregion

        // ##############################################################################################################################
        // Constructor
        // ##############################################################################################################################

        #region Constructor

        public NLogViewer()
        {
            InitializeComponent();
            DataContext = this;

            // save instance UID
            Uid = GetHashCode().ToString();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            LogEvents = new CollectionViewSource { Source = _LogEventInfos };

            Loaded += _OnLoaded;
            Unloaded += _OnUnloaded;
            ClearCommand = new ActionCommand(_LogEventInfos.Clear);
        }

        private void _OnUnloaded(object sender, RoutedEventArgs e)
        {
            // look in logical and visual tree if the control has been removed
            // If there is no parent window found before, we have a special case (https://github.com/dojo90/NLogViewer/issues/30) and just dispose it anyway
            if (_ParentWindow.FindChildByUid<NLogViewer>(Uid) == null)
            {
                _Dispose();
            }
        }

        private void _ParentWindowOnClosed(object sender, EventArgs e)
        {
            _Dispose();
        }

        private void _Dispose()
        {
            _Subscription?.Dispose();
        }

        private void _OnLoaded(object sender, RoutedEventArgs e)
        {
            // removed loaded handler to prevent duplicate subscribing
            Loaded -= _OnLoaded;

            // add hook to parent window to dispose subscription
            // use case:
            // NLogViewer is used in a new window inside of a TabControl. If you switch the TabItems,
            // the unloaded event is called and would dispose the subscription, even if the control is still alive.
            if (Window.GetWindow(this) is { } window)
            {
                _ParentWindow = window;
                _ParentWindow.Closed += _ParentWindowOnClosed;
            }

            ListView.ScrollToEnd();
            var target = CacheTarget.GetInstance(targetName: TargetName);

            _Subscription = target.Cache.SubscribeOn(Scheduler.Default).Buffer(TimeSpan.FromMilliseconds(100)).Where(x => x.Any()).ObserveOn(new DispatcherSynchronizationContext(_ParentWindow.Dispatcher)).Subscribe(infos =>
            {
                if (Pause) return;
                using (LogEvents.DeferRefresh())
                {
                    foreach (LogEventInfo info in infos)
                    {
                        _LogEventInfos.Add(info);
                    }
                    if (MaxCount >= 0 & _LogEventInfos.Count - 100 > MaxCount)
                    {
                        for (int i = 0; i < _LogEventInfos.Count - MaxCount; i++)
                        {
                            _LogEventInfos.RemoveAt(0);
                        }
                    }
                }

                if (AutoScroll)
                {
                    ListView?.ScrollToEnd();
                }
            });
        }

        public void AddEventInfo(LogEventInfo logEvent)
        {
            _LogEventInfos.Add(logEvent);
        }

        public bool HasEvents()
        {
            return _LogEventInfos.Any();
        }

        public void LoadLines(List<string> lines, DateTime? fromDateTime = null)
        {
            _LogEventInfos.Clear();

            var lastEventInfo = default(LogEventInfo);

            foreach (var line in lines)
            {
                var parts = line.Split("|");

                if (parts.Length < 4)
                {
                    if (lastEventInfo != null && !string.IsNullOrWhiteSpace(line))
                    {
                        lastEventInfo.Message += "\r\n" + line;

                        continue;
                    }

                    if (lines.Count > 1)
                    {
                        continue;
                    }

                    parts = new string[] { DateTime.Now.ToString(), "info", "", line };
                }

                var timeStamp = DateTime.Parse(parts[0]);

                if (fromDateTime > timeStamp)
                {
                    continue;
                }

                var logLevel = LogLevel.FromString(parts[1]);

                var logEvent = new LogEventInfo(logLevel, parts[2], parts[3]);
                logEvent.TimeStamp = timeStamp;

                lastEventInfo = logEvent;

                AddEventInfo(logEvent);
            }
        }

        #endregion
    }
}
