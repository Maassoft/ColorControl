using System;
using System.Windows;
using System.Windows.Controls;

namespace DJ.Helper.ListViewLayoutManager
{
    public abstract class LayoutColumn
    {
        protected static bool HasPropertyValue(GridViewColumn column, DependencyProperty dp)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            object value = column.ReadLocalValue(dp);
            if (value?.GetType() == dp.PropertyType)
            {
                return true;
            }

            return false;
        }
        
        protected static double? GetColumnWidth(GridViewColumn column, DependencyProperty dp)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            object value = column.ReadLocalValue(dp);
            if (value?.GetType() == dp.PropertyType)
            {
                return (double) value;
            }

            return null;
        }
    }
}
