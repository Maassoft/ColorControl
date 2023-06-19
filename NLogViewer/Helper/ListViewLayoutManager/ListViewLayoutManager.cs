using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DJ.Helper.ListViewLayoutManager
{
    public class ListViewLayoutManager
    {
        // ##############################################################################################################################
        // Properties
        // ##############################################################################################################################

        #region Properties

        // ##########################################################################################
        // Public Properties
        // ##########################################################################################

        public ListView ListView => _ListView;

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => _VerticalScrollBarVisibility;
            set => _VerticalScrollBarVisibility = value;
        }

        // ##########################################################################################
        // Private Properties
        // ##########################################################################################

        private readonly ListView _ListView;
        private ScrollViewer _ScrollViewer;
        private bool _Loaded;
        private bool _Resizing;
        private Cursor _ResizeCursor;
        private ScrollBarVisibility _VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        private GridViewColumn _AutoSizedColumn;

        private const double _ZERO_WIDTH_RANGE = 0.1;

        #endregion

        // ##############################################################################################################################
        // AttachedProperties
        // ##############################################################################################################################

        #region AttachedProperties

        public static void SetEnabled(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(EnabledProperty, enabled);
        }

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled",
            typeof(bool), typeof(ListViewLayoutManager), new FrameworkPropertyMetadata(_OnLayoutManagerEnabledChanged));

        #endregion

        // ##############################################################################################################################
        // Constructor
        // ##############################################################################################################################

        #region Constructor

        public ListViewLayoutManager(ListView listView)
        {
            _ListView = listView ?? throw new ArgumentNullException(nameof(listView));
            _ListView.Loaded += ListView_Loaded;
            _ListView.Unloaded += ListView_Unloaded;
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            _RegisterEvents(_ListView);
            _InitColumns();
            _DoResizeColumns();
            _Loaded = true;
        }


        private void ListView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!_Loaded)
            {
                return;
            }

            _UnRegisterEvents(_ListView);
            _Loaded = false;
        }

        #endregion

        // ##############################################################################################################################
        // public methods
        // ##############################################################################################################################

        #region public methods

        public void Refresh()
        {
            _InitColumns();
            _DoResizeColumns();
        }

        protected virtual void ResizeColumns()
        {
            GridView view = _ListView.View as GridView;
            if (view == null || view.Columns.Count == 0)
            {
                return;
            }

            // listview width
            double actualWidth = double.PositiveInfinity;
            if (_ScrollViewer != null)
            {
                actualWidth = _ScrollViewer.ViewportWidth;
            }

            if (double.IsInfinity(actualWidth))
            {
                actualWidth = _ListView.ActualWidth;
            }

            if (double.IsInfinity(actualWidth) || actualWidth <= 0)
            {
                return;
            }

            double resizeableRegionCount = 0;
            double otherColumnsWidth = 0;
            // determine column sizes
            foreach (GridViewColumn gridViewColumn in view.Columns)
            {
                if (ProportionalColumn.IsProportionalColumn(gridViewColumn))
                {
                    double? proportionalWidth = ProportionalColumn.GetProportionalWidth(gridViewColumn);
                    if (proportionalWidth != null)
                    {
                        resizeableRegionCount += proportionalWidth.Value;
                    }
                }
                else
                {
                    otherColumnsWidth += gridViewColumn.ActualWidth;
                }
            }

            if (resizeableRegionCount <= 0)
            {
                // no proportional columns present: commit the regulation to the scroll viewer
                if (_ScrollViewer != null)
                {
                    _ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                }

                // search the first fill column
                GridViewColumn fillColumn = null;
                for (int i = 0; i < view.Columns.Count; i++)
                {
                    GridViewColumn gridViewColumn = view.Columns[i];
                    if (_IsFillColumn(gridViewColumn))
                    {
                        fillColumn = gridViewColumn;
                        break;
                    }
                }

                if (fillColumn != null)
                {
                    double otherColumnsWithoutFillWidth = otherColumnsWidth - fillColumn.ActualWidth;
                    double fillWidth = actualWidth - otherColumnsWithoutFillWidth;
                    if (fillWidth > 0)
                    {
                        double? minWidth = RangeColumn.GetRangeMinWidth(fillColumn);
                        double? maxWidth = RangeColumn.GetRangeMaxWidth(fillColumn);

                        bool setWidth = !(minWidth.HasValue && fillWidth < minWidth.Value);
                        if (maxWidth.HasValue && fillWidth > maxWidth.Value)
                        {
                            setWidth = false;
                        }

                        if (setWidth)
                        {
                            if (_ScrollViewer != null)
                            {
                                _ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                            }

                            fillColumn.Width = fillWidth;
                        }
                    }
                }

                return;
            }

            double resizeableColumnsWidth = actualWidth - otherColumnsWidth;
            if (resizeableColumnsWidth <= 0)
            {
                return; // missing space
            }

            // resize columns
            double resizeableRegionWidth = resizeableColumnsWidth / resizeableRegionCount;
            foreach (GridViewColumn gridViewColumn in view.Columns)
            {
                if (ProportionalColumn.IsProportionalColumn(gridViewColumn))
                {
                    double? proportionalWidth = ProportionalColumn.GetProportionalWidth(gridViewColumn);
                    if (proportionalWidth != null)
                    {
                        gridViewColumn.Width = proportionalWidth.Value * resizeableRegionWidth;
                    }
                }
            }
        }

        #endregion

        // ##############################################################################################################################
        // private methods
        // ##############################################################################################################################

        #region private methods

        private void _DoResizeColumns()
        {
            if (_Resizing)
            {
                return;
            }

            _Resizing = true;
            try
            {
                ResizeColumns();
            }
            finally
            {
                _Resizing = false;
            }
        }

        private void _RegisterEvents(DependencyObject start)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                Visual childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                if (childVisual is Thumb)
                {
                    GridViewColumn gridViewColumn = _FindParentColumn(childVisual);
                    if (gridViewColumn != null)
                    {
                        Thumb thumb = childVisual as Thumb;
                        if (ProportionalColumn.IsProportionalColumn(gridViewColumn) ||
                            FixedColumn.IsFixedColumn(gridViewColumn) || _IsFillColumn(gridViewColumn))
                        {
                            thumb.IsHitTestVisible = false;
                        }
                        else
                        {
                            thumb.PreviewMouseMove += _ThumbPreviewMouseMove;
                            thumb.PreviewMouseLeftButtonDown +=
                                _ThumbPreviewMouseLeftButtonDown;
                            DependencyPropertyDescriptor.FromProperty(
                                GridViewColumn.WidthProperty,
                                typeof(GridViewColumn)).AddValueChanged(gridViewColumn, _GridColumnWidthChanged);
                        }
                    }
                }
                else if (childVisual is GridViewColumnHeader)
                {
                    GridViewColumnHeader columnHeader = childVisual as GridViewColumnHeader;
                    columnHeader.SizeChanged += _GridColumnHeaderSizeChanged;
                }
                else if (_ScrollViewer == null && childVisual is ScrollViewer)
                {
                    _ScrollViewer = childVisual as ScrollViewer;
                    _ScrollViewer.ScrollChanged += _ScrollViewerScrollChanged;
                    // assume we do the regulation of the horizontal scrollbar
                    _ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    _ScrollViewer.VerticalScrollBarVisibility = _VerticalScrollBarVisibility;
                }

                _RegisterEvents(childVisual);
            }
        }

        private void _UnRegisterEvents(DependencyObject start)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                Visual childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                if (childVisual is Thumb)
                {
                    GridViewColumn gridViewColumn = _FindParentColumn(childVisual);
                    if (gridViewColumn != null)
                    {
                        Thumb thumb = childVisual as Thumb;
                        if (ProportionalColumn.IsProportionalColumn(gridViewColumn) ||
                            FixedColumn.IsFixedColumn(gridViewColumn) || _IsFillColumn(gridViewColumn))
                        {
                            thumb.IsHitTestVisible = true;
                        }
                        else
                        {
                            thumb.PreviewMouseMove -= _ThumbPreviewMouseMove;
                            thumb.PreviewMouseLeftButtonDown -=
                                _ThumbPreviewMouseLeftButtonDown;
                            DependencyPropertyDescriptor.FromProperty(
                                GridViewColumn.WidthProperty,
                                typeof(GridViewColumn)).RemoveValueChanged(gridViewColumn, _GridColumnWidthChanged);
                        }
                    }
                }
                else if (childVisual is GridViewColumnHeader)
                {
                    GridViewColumnHeader columnHeader = childVisual as GridViewColumnHeader;
                    columnHeader.SizeChanged -= _GridColumnHeaderSizeChanged;
                }
                else if (_ScrollViewer == null && childVisual is ScrollViewer)
                {
                    _ScrollViewer = childVisual as ScrollViewer;
                    _ScrollViewer.ScrollChanged -= _ScrollViewerScrollChanged;
                }

                _UnRegisterEvents(childVisual);
            }
        }

        private GridViewColumn _FindParentColumn(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            while (element != null)
            {
                GridViewColumnHeader gridViewColumnHeader = element as GridViewColumnHeader;
                if (gridViewColumnHeader != null)
                {
                    return (gridViewColumnHeader).Column;
                }

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }

        private GridViewColumnHeader _FindColumnHeader(DependencyObject start, GridViewColumn gridViewColumn)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                Visual childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                if (childVisual is GridViewColumnHeader)
                {
                    GridViewColumnHeader gridViewHeader = childVisual as GridViewColumnHeader;
                    if (gridViewHeader.Column == gridViewColumn)
                    {
                        return gridViewHeader;
                    }
                }

                GridViewColumnHeader childGridViewHeader = _FindColumnHeader(childVisual, gridViewColumn);
                if (childGridViewHeader != null)
                {
                    return childGridViewHeader;
                }
            }

            return null;
        }

        private void _InitColumns()
        {
            GridView view = _ListView.View as GridView;
            if (view == null)
            {
                return;
            }

            foreach (GridViewColumn gridViewColumn in view.Columns)
            {
                if (!RangeColumn.IsRangeColumn(gridViewColumn))
                {
                    continue;
                }

                double? minWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
                double? maxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);
                if (!minWidth.HasValue && !maxWidth.HasValue)
                {
                    continue;
                }

                GridViewColumnHeader columnHeader = _FindColumnHeader(_ListView, gridViewColumn);
                if (columnHeader == null)
                {
                    continue;
                }

                double actualWidth = columnHeader.ActualWidth;
                if (minWidth.HasValue)
                {
                    columnHeader.MinWidth = minWidth.Value;
                    if (!double.IsInfinity(actualWidth) && actualWidth < columnHeader.MinWidth)
                    {
                        gridViewColumn.Width = columnHeader.MinWidth;
                    }
                }

                if (maxWidth.HasValue)
                {
                    columnHeader.MaxWidth = maxWidth.Value;
                    if (!double.IsInfinity(actualWidth) && actualWidth > columnHeader.MaxWidth)
                    {
                        gridViewColumn.Width = columnHeader.MaxWidth;
                    }
                }
            }
        }

        // returns the delta
        private double _SetRangeColumnToBounds(GridViewColumn gridViewColumn)
        {
            double startWidth = gridViewColumn.Width;

            double? minWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
            double? maxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);

            if ((minWidth.HasValue && maxWidth.HasValue) && (minWidth > maxWidth))
            {
                return 0; // invalid case
            }

            if (minWidth.HasValue && gridViewColumn.Width < minWidth.Value)
            {
                gridViewColumn.Width = minWidth.Value;
            }
            else if (maxWidth.HasValue && gridViewColumn.Width > maxWidth.Value)
            {
                gridViewColumn.Width = maxWidth.Value;
            }

            return gridViewColumn.Width - startWidth;
        }

        private bool _IsFillColumn(GridViewColumn gridViewColumn)
        {
            if (gridViewColumn == null)
            {
                return false;
            }

            GridView view = _ListView.View as GridView;
            if (view == null || view.Columns.Count == 0)
            {
                return false;
            }

            bool? isFillColumn = RangeColumn.GetRangeIsFillColumn(gridViewColumn);
            return isFillColumn.HasValue && isFillColumn.Value;
        }

        private void _ThumbPreviewMouseMove(object sender, MouseEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            if (thumb == null)
            {
                return;
            }

            GridViewColumn gridViewColumn = _FindParentColumn(thumb);
            if (gridViewColumn == null)
            {
                return;
            }

            // suppress column resizing for proportional, fixed and range fill columns
            if (ProportionalColumn.IsProportionalColumn(gridViewColumn) ||
                FixedColumn.IsFixedColumn(gridViewColumn) ||
                _IsFillColumn(gridViewColumn))
            {
                thumb.Cursor = null;
                return;
            }

            // check range column bounds
            if (thumb.IsMouseCaptured && RangeColumn.IsRangeColumn(gridViewColumn))
            {
                double? minWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
                double? maxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);

                if ((minWidth.HasValue && maxWidth.HasValue) && (minWidth > maxWidth))
                {
                    return; // invalid case
                }

                if (_ResizeCursor == null)
                {
                    _ResizeCursor = thumb.Cursor; // save the resize cursor
                }

                if (minWidth.HasValue && gridViewColumn.Width <= minWidth.Value)
                {
                    thumb.Cursor = Cursors.No;
                }
                else if (maxWidth.HasValue && gridViewColumn.Width >= maxWidth.Value)
                {
                    thumb.Cursor = Cursors.No;
                }
                else
                {
                    thumb.Cursor = _ResizeCursor; // between valid min/max
                }
            }
        }

        private void _ThumbPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            GridViewColumn gridViewColumn = _FindParentColumn(thumb);

            // suppress column resizing for proportional, fixed and range fill columns
            if (ProportionalColumn.IsProportionalColumn(gridViewColumn) ||
                FixedColumn.IsFixedColumn(gridViewColumn) ||
                _IsFillColumn(gridViewColumn))
            {
                e.Handled = true;
            }
        }

        private void _GridColumnWidthChanged(object sender, EventArgs e)
        {
            if (!_Loaded)
            {
                return;
            }

            GridViewColumn gridViewColumn = sender as GridViewColumn;

            // suppress column resizing for proportional and fixed columns
            if (ProportionalColumn.IsProportionalColumn(gridViewColumn) || FixedColumn.IsFixedColumn(gridViewColumn))
            {
                return;
            }

            // ensure range column within the bounds
            if (RangeColumn.IsRangeColumn(gridViewColumn))
            {
                // special case: auto column width - maybe conflicts with min/max range
                if (gridViewColumn != null && gridViewColumn.Width.Equals(double.NaN))
                {
                    _AutoSizedColumn = gridViewColumn;
                    return; // handled by the change header size event
                }

                // ensure column bounds
                if (Math.Abs(_SetRangeColumnToBounds(gridViewColumn) - 0) > _ZERO_WIDTH_RANGE)
                {
                    return;
                }
            }

            _DoResizeColumns();
        }

        // handle autosized column
        private void _GridColumnHeaderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_AutoSizedColumn == null)
            {
                return;
            }

            GridViewColumnHeader gridViewColumnHeader = sender as GridViewColumnHeader;
            if (gridViewColumnHeader != null && gridViewColumnHeader.Column == _AutoSizedColumn)
            {
                if (gridViewColumnHeader.Width.Equals(double.NaN))
                {
                    // sync column with 
                    gridViewColumnHeader.Column.Width = gridViewColumnHeader.ActualWidth;
                    _DoResizeColumns();
                }

                _AutoSizedColumn = null;
            }
        }

        private void _ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_Loaded && Math.Abs(e.ViewportWidthChange - 0) > _ZERO_WIDTH_RANGE)
            {
                _DoResizeColumns();
            }
        }

        private static void _OnLayoutManagerEnabledChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            ListView listView = dependencyObject as ListView;
            if (listView != null)
            {
                bool enabled = (bool) e.NewValue;
                if (enabled)
                {
                    new ListViewLayoutManager(listView);
                }
            }
        }

        #endregion
    }
}