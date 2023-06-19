using System.Windows;
using System.Windows.Controls;

namespace DJ.Helper.ListViewLayoutManager
{
    public sealed class ProportionalColumn : LayoutColumn
    {
        public static bool IsProportionalColumn(GridViewColumn column)
        {
            if (column == null)
            {
                return false;
            }

            return HasPropertyValue(column, WidthProperty);
        }

        public static double? GetProportionalWidth(GridViewColumn column)
        {
            return GetColumnWidth(column, WidthProperty);
        }

        public static GridViewColumn ApplyWidth(GridViewColumn gridViewColumn, double width)
        {
            SetWidth(gridViewColumn, width);
            return gridViewColumn;
        }

        // ##############################################################################################################################
        // AttachedProperties
        // ##############################################################################################################################

        #region AttachedProperties

        public static double GetWidth(DependencyObject obj)
        {
            return (double) obj.GetValue(WidthProperty);
        }

        public static void SetWidth(DependencyObject obj, double width)
        {
            obj.SetValue(WidthProperty, width);
        }

        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached("Width",  typeof(double),  typeof(ProportionalColumn));

        #endregion

        // ##############################################################################################################################
        // Constructor
        // ##############################################################################################################################

        #region Constructor

        private ProportionalColumn()
        {
        }

        #endregion
    }
}