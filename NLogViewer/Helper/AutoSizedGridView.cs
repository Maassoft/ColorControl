using System;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Controls;
using NLog;

namespace DJ.Helper
{
    /// <summary>
    /// Represents a view mode that displays data items in columns for a System.Windows.Controls.ListView control with auto sized columns based on the column content
    /// Used to fix the column width: https://stackoverflow.com/questions/60147905/column-width-adjustment-is-broken-if-using-multibinding-on-displaymemberbinding
    /// </summary>
    public class AutoSizedGridView : GridView
    {
        private int _MaxLoggerNameLength;

        protected override void PrepareItem(ListViewItem item)
        {
            if (item.DataContext is LogEventInfo info)
            {
                if (info.LoggerName.Length > _MaxLoggerNameLength)
                {
                    _MaxLoggerNameLength = info.LoggerName.Length;
                    Observable.Timer(TimeSpan.FromMilliseconds(1)).ObserveOn(SynchronizationContext.Current).Subscribe(l =>
                    {
                        foreach (GridViewColumn column in Columns)
                        {
                            //setting NaN for the column width automatically determines the required width enough to hold the content completely.
                            //if column width was set to NaN already, set it ActualWidth temporarily and set to NaN. This raises the property change event and re computes the width.
                            if (double.IsNaN(column.Width))
                            {
                                column.Width = column.ActualWidth;
                                column.Width = double.NaN;
                            }
                        }
                    });
                }
            }
            
            base.PrepareItem(item);
        }
    }
}
